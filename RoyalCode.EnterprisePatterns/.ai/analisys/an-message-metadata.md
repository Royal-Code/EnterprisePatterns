# Metadados persistidos da Outbox

## Estado atual

`OutboxMessage` persiste `Id`, `CreatedAt`, `MessageType`, `VersionType`, `Key` e `Payload`. O contrato `CreateMessage` aceita `Key`, mas `CreateMessageHandler` nao a copia para a entidade. `CreatedAt` e definido na criacao da mensagem; ele nao preserva necessariamente o momento em que o evento ocorreu.

O `IDomainEvent.Id` e `IDomainEvent.Occurred` existem no payload, mas nao sao campos consultaveis. Isso dificulta idempotencia do Inbox, observabilidade, roteamento e diagnostico sem desserializar o conteudo.

## Metadados minimos propostos

| Campo | Finalidade | Regra |
| --- | --- | --- |
| `Id` | identidade tecnica da Outbox | sequencial, ja existente |
| `EventId` | identidade estavel do evento | `Guid`, para Inbox e correlacao |
| `OccurredAt` | instante de negocio | `DateTimeOffset`, vindo do evento |
| `CreatedAt` | instante de persistencia | `DateTimeOffset`, UTC |
| `MessageType` + `VersionType` | contrato | par imutavel |
| `Key` | chave de particao/roteamento | opcional, com semantica documentada |
| `CorrelationId` | correlacao entre operacoes | opcional e de baixa cardinalidade |
| `CausationId` | mensagem/evento que causou esta | opcional |
| protecao do payload | modo e chave | detalhado em [an-payload-protection.md](an-payload-protection.md) |

`Key` deve ser copiada imediatamente pelo `CreateMessageHandler`. O nome pode ser mantido para compatibilidade, mas a documentacao deve defini-la como chave de particao, e nao como campo generico sem uso.

## Fronteira da API

O Outbox deve preferir eventos que implementem `IDomainEvent`, pois eles fornecem `EventId` e `OccurredAt`. Para manter `IOutboxService.Write(object)` como extensao generica, ha duas opcoes:

1. Exigir `IDomainEvent` no caminho transacional e oferecer uma API explicita para mensagens externas.
2. Criar `OutboxMessageData`, que recebe os metadados obrigatorios de uma mensagem externa e e validado antes de persistir.

A primeira opcao e a mais simples para eventos de dominio. A segunda e necessaria se a biblioteca tambem deve registrar comandos de integracao que nao sao eventos de dominio.

## Evolucao de schema

Adicionar inicialmente colunas anulaveis para registros existentes e preencher todos os novos registros. Uma mensagem legada sem `EventId` deve ser marcada/tratada como legado na borda de transporte; nao se deve tentar extrair o Id do JSON de forma generica.

Depois do periodo de retencao dos registros antigos, tornar os campos obrigatorios em uma migracao posterior. Criar indices para `event_id` e, somente se houver consulta real, para `key`. Nao criar unicidade em `EventId`: replay operacional pode publicar de novo o mesmo evento com um novo Id tecnico de Outbox.

## Decisao recomendada

Corrigir a copia de `Key` como ajuste imediato. Em seguida, evoluir o modelo para `EventId`, `OccurredAt`, correlacao e causacao antes do transporte. Esses dados formam o envelope estavel que o Inbox, RabbitMQ e Kafka poderao transportar sem abrir o payload.