# Confirmacao de consumo e concorrencia

## Estado atual

Cada `OutboxConsumer` possui um nome unico e `LastConsumedMessageId`. A leitura busca mensagens com `Id` maior que esse valor, em ordem crescente. O commit recebe um valor arbitrario e o persiste diretamente no consumidor.

Isso e eficiente para leitura: indice primario de `OutboxMessage.Id`, uma consulta por pagina e uma atualizacao pequena do cursor. Tambem mantem uma linha do tempo por consumidor, sem copiar cada mensagem para cada consumidor.

## Problema

O commit atual nao prova que a mensagem foi publicada, aceita saltos e nao coordena duas instancias do mesmo dispatcher. Assim, uma chamada pode avancar o cursor para uma mensagem inexistente ou para uma mensagem posterior a outras ainda nao processadas.

Ha ainda uma sutileza importante em produtores concorrentes: uma transacao pode receber o `Id` 10, ficar aberta, e outra gravar e confirmar o `Id` 11. Um cursor que confirma 11 antes de 10 ficar visivel perde 10 quando a primeira transacao confirmar. Logo, um cursor de maior Id nao pode ser a fonte de verdade de entrega quando ha transacoes concorrentes e nao existe serializacao da escrita.

O `Id` continua util para pagina e para a ordem das mensagens visiveis. Ele nao deve prometer ordem global de commit entre transacoes concorrentes.

## Arquitetura proposta

Separar a API administrativa de cursor da confirmacao usada pelo dispatcher confiavel.

```text
OutboxConsumerReader
  -> mensagens sem confirmacao para o consumidor, por Id crescente
IOutboxTransportPublisher
  -> publica uma mensagem
IOutboxConsumerAcknowledger
  -> grava a confirmacao exata da mensagem apos publicar
```

Criar uma tabela enxuta `outbox_consumer_messages`:

| Coluna | Regra |
| --- | --- |
| `consumer_id` | FK/logical id do consumidor |
| `message_id` | Id da Outbox |
| `consumed_at` | instante da confirmacao |

Chave primaria composta: `(consumer_id, message_id)`. A leitura faz `NOT EXISTS` nessa tabela, ordena por `outbox_messages.id` e usa `LIMIT`. O indice da chave composta atende o anti-join; `outbox_messages.id` ja e o indice de varredura.

Depois de `PublishAsync` sem excecao, a confirmacao faz um `INSERT` da dupla `(consumer_id, message_id)`. A colisao de chave significa que outra tentativa ja havia confirmado a mesma entrega e e tratada como sucesso. A publicacao pode ocorrer mais de uma vez entre o broker e esse insert; essa e a garantia correta de pelo menos uma vez.

`LastConsumedMessageId` pode ser mantido temporariamente como checkpoint observavel ou migrado para fora do caminho confiavel. Ele nao participa da decisao de elegibilidade. O endpoint HTTP atual deve ser documentado como administrativo/legado ou substituido por um endpoint de acknowledgement por `messageId`; ele nao deve ser usado pelo dispatcher.

## Lideranca e fencing

O acknowledger confiavel recebe tambem uma lease valida e um fencing token. O insert de confirmacao deve verificar a lease na mesma operacao SQL. Se a instancia perdeu a lideranca, ela nao confirma o progresso. A publicacao anterior pode duplicar, mas nunca vira perda de mensagem.

O design da lease esta em [an-dispatcher-leadership.md](an-dispatcher-leadership.md).

## Alternativas

### Cursor com precondicao de proxima mensagem

Atualizar o cursor somente se o Id for a proxima mensagem visivel reduz saltos e e muito barato. Ainda falha no caso de uma transacao antiga, com Id menor, confirmar depois de um Id maior ja consumido. So e aceitavel se o sistema serializar a alocacao/confirmacao de mensagens, o que adiciona contencao global.

### Ledger de confirmacoes

E a opcao recomendada. Custa uma linha por mensagem entregue por consumidor, mas oferece semantica correta para produtores concorrentes, novos consumidores e reprocessamento. O custo e previsivel, indexavel e combina com uma politica explicita de retencao.

## Decisao recomendada

Usar o ledger de confirmacoes como fonte de verdade para dispatchers. Processar uma mensagem por vez na primeira versao, em ordem das mensagens visiveis, e confirmar apenas apos a publicacao. Nao prometer ordenacao global entre transacoes concorrentes; quando for necessaria ordem por agregado, usar uma chave de particao e um protocolo proprio por chave.