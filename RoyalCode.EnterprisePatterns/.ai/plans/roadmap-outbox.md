# Roadmap do Outbox

Este roadmap parte da implementacao atual e das analises em [../analisys](../analisys). Ele prioriza garantir que nenhuma mensagem confirmada seja perdida antes de introduzir brokers ou sofisticacao operacional.

## 1. Consolidar a base atual

- Manter as correcoes ja feitas para ciclo de vida de escopo e `JsonTypeInfo`.
- Adicionar testes focados de serializacao, escrita no mesmo `DbContext` e dispatch local.
- Documentar a fronteira de unidade de trabalho descrita em [an-domain-event-lifecycle.md](../analisys/an-domain-event-lifecycle.md).

## 2. Estabilizar o envelope persistido

- Corrigir a copia de `CreateMessage.Key` para `OutboxMessage`.
- Adicionar `EventId`, `OccurredAt`, correlacao e causacao, com migracao compativel para registros legados.
- Definir a semantica de `Key` como chave de particao/roteamento.
- Usar [an-message-metadata.md](../analisys/an-message-metadata.md) como criterio de schema e testes.

## 3. Projetar contratos versionados

- Substituir o registro unico por tipo por indices de leitura `(contract, version)` e escrita por CLR type.
- Manter `AddMessageType<T>(name)` como atalho v1 e introduzir a API explicita de versao/publicacao.
- Validar colisoes no startup e testar coexistencia/migracao de v1 para v2.
- Seguir [an-contract-versioning.md](../analisys/an-contract-versioning.md).

## 4. Adicionar protecao de payload

- Persistir modo de protecao e identificador de chave junto do envelope.
- Implementar o codec como integracao opcional e testar payload aberto, protegido, chave rotacionada e mensagem legada.
- Nao permitir que o futuro publisher abra ou altere ciphertext.
- Seguir [an-payload-protection.md](../analisys/an-payload-protection.md).

## 5. Tornar o consumo duravel

- Introduzir o ledger `(consumer_id, message_id)` e reader de mensagens nao confirmadas.
- Migrar o endpoint de commit arbitrario para acknowledgement exato ou marca-lo como legado administrativo.
- Definir a retencao de mensagens e acknowledgements antes de expor consumidores que iniciam no historico inteiro.
- Validar produtores concorrentes com commits fora de ordem, retries e novo consumidor.
- Seguir [an-consumer-cursor-commit.md](../analisys/an-consumer-cursor-commit.md).

## 6. Coordenar multiplas instancias

- Criar a tabela de lease, o coordinator PostgreSQL e o fencing token.
- Fazer o acknowledgement exigir a lease no mesmo comando atomico.
- Testar troca de lider, expiracao, pausa longa e tentativa de commit por lider antigo.
- Seguir [an-dispatcher-leadership.md](../analisys/an-dispatcher-leadership.md).

## 7. Entregar o dispatcher generico

- Criar o worker com escopos curtos, lote unitario inicial e publish antes de acknowledgement.
- Integrar retry em memoria com backoff e jitter; parar o lote na primeira falha.
- Testar cancelamento, reinicio e duplicata entre publish e acknowledgement.
- Seguir [an-dispatcher-worker.md](../analisys/an-dispatcher-worker.md) e [an-dispatcher-retry.md](../analisys/an-dispatcher-retry.md).

## 8. Adicionar o primeiro adaptador de mensageria

- Criar `IOutboxTransportPublisher`, envelope opaco e registro de rotas por contrato/versao.
- Implementar RabbitMQ primeiro; Kafka fica como adaptador posterior sob a mesma fronteira.
- Testar ausencia de rota, confirmacao do cliente e preservacao de payload protegido.
- Seguir [an-transport-publisher.md](../analisys/an-transport-publisher.md).

## 9. Fechar a operacao

- Adicionar metricas, logs sem payload, traces e health checks ao dispatcher.
- Definir limites operacionais para backlog, idade da mensagem e falhas consecutivas.
- Reavaliar retry persistido/poison do produtor somente se houver requisito operacional concreto.
- Seguir [an-dispatcher-observability.md](../analisys/an-dispatcher-observability.md).

## Resultado esperado

Ao final, a Outbox tera escrita transacional, contratos evolutiveis, payload protegido quando necessario, entrega pelo menos uma vez por consumidor, lideranca segura em multiplas instancias e adaptadores de transporte sem acoplamento ao dominio. O Inbox pode entao usar `EventId` e o envelope estavel para implementar idempotencia sem redesenhar a Outbox.