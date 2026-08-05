# Observabilidade do dispatcher

## Objetivo

Oferecer sinal operacional suficiente para detectar atraso, falhas e perda de lideranca sem registrar payloads ou criar tags de alta cardinalidade.

## Metricas iniciais

Usar `System.Diagnostics.Metrics` no pacote de dispatching:

| Metrica | Tipo | Tags permitidas |
| --- | --- | --- |
| `outbox.publish.count` | counter | dispatcher, consumer, contract, version, outcome |
| `outbox.publish.duration` | histogram | dispatcher, consumer, contract, version |
| `outbox.lease.transition.count` | counter | dispatcher, transition |
| `outbox.pending.count` | observable gauge | dispatcher, consumer |
| `outbox.oldest.pending.age` | observable gauge | dispatcher, consumer |

Nunca usar `MessageId`, `EventId`, chave de particao, texto de excecao ou payload como tag. `contract` e `version` possuem cardinalidade limitada pela configuracao de startup.

## Logs e traces

Usar logs estruturados para aquisicao/perda de lease, inicio de retry, falha de rota e confirmacao. Registrar apenas identidade tecnica, contrato, versao e tentativa. O payload nao deve aparecer em logs, mesmo quando estiver aberto.

Criar uma `ActivitySource` para cada tentativa de publish. O contexto de correlacao segue no envelope e pode ser propagado pelo adaptador de transporte, sem exigir que a Outbox interprete o payload.

## Health

Readiness deve falhar para configuracao invalida de contrato, rota ou codec protegido ausente. Liveness deve indicar que o worker ainda executa ciclos; uma instancia follower sem lease continua saudavel. Atraso excessivo ou falhas consecutivas devem degradar readiness conforme limite configurado, sem tornar um follower automaticamente nao saudavel.

## Performance

Calcular backlog e idade em uma amostragem periodica com consulta agregada e armazenar o ultimo resultado para o observable gauge. Nao executar `COUNT(*)` em toda publicacao e nao incluir tracing detalhado quando a instrumentacao estiver desabilitada.

## Decisao

Entregar metricas, logs seguros e health junto do worker. Manter retry detalhado e dashboards como consumidores dessas telemetrias, nao como dependencias do caminho de publicacao.