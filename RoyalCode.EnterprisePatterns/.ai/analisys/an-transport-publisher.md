# Fronteira de transporte

## Objetivo

Publicar registros persistidos da Outbox em RabbitMQ, Kafka ou outro broker sem acoplar `OutboxService` a um protocolo e sem desserializar o payload no produtor de transporte.

## Design recomendado

Criar no nucleo a fronteira `IOutboxTransportPublisher` e um envelope imutavel de transporte. O publisher recebe uma representacao somente leitura da mensagem persistida e retorna sucesso apenas quando o cliente do broker aceita a publicacao.

```text
OutboxMessage
  -> OutboxTransportEnvelope
  -> IOutboxTransportPublisher.PublishAsync
  -> adaptador RabbitMQ, Kafka ou outro
```

O envelope carrega `SourceMessageId`, `EventId`, contrato, versao, datas, chave de particao, correlacao, metadados de protecao e o payload original. Ele nao carrega uma instancia CLR do evento.

Cada adaptador mantem um registro de rota por `(MessageType, VersionType)`. Uma rota RabbitMQ resolve exchange/routing key e uma rota Kafka resolve topic/key. Ausencia de rota e erro de configuracao que deve falhar cedo no startup ou, no maximo, bloquear a mensagem sem confirma-la.

## Pacotes

| Pacote | Responsabilidade |
| --- | --- |
| `RoyalCode.Events.Outbox.Abstractions` | envelope e interface do publisher |
| `RoyalCode.Events.Outbox.Dispatching` | orquestra leitura, publish e acknowledgement |
| `RoyalCode.Events.Outbox.RabbitMQ` | rotas e cliente AMQP |
| `RoyalCode.Events.Outbox.Kafka` | rotas e produtor Kafka, quando necessario |

Os adaptadores devem depender do nucleo, nunca o contrario. `IMessageDispatcher` continua reservado ao dispatch local tipado e nao e usado para enviar ao broker.

## Performance e confiabilidade

O adaptador envia o payload persistido como bytes UTF-8 e usa o pool/conexao do cliente oficial do broker. Nao abre `DbContext`, nao cria escopo por serializacao e nao faz decrypt. Excecoes ou confirmacoes negativas retornam como falha para o worker, que deixa a mensagem sem acknowledgement.

## Decisao

Comecar com um envelope opaco e uma rota por contrato/versao. Isso permite RabbitMQ primeiro e Kafka depois sem alterar o schema, a serializacao ou a semantica de entrega da Outbox.