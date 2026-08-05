# Worker de despacho

## Objetivo

Executar o polling, a publicacao e a confirmacao de consumo sem manter conexoes de banco abertas durante chamadas de rede e sem acoplar o nucleo a `BackgroundService`.

## Design recomendado

Criar um pacote de dispatching com `IOutboxDispatcher` e uma adaptacao hospedada que use `BackgroundService`. O host registra o worker e fornece configuracao; o nucleo continua utilizavel por jobs, consoles e testes.

```text
obter ou renovar lease
  -> ler lote de mensagens elegiveis sem tracking
  -> para cada mensagem, em ordem visivel:
       publicar no transporte
       confirmar a mensagem para o consumidor
  -> aguardar proximo ciclo
```

O worker cria um escopo curto para leitura e outro para acknowledgement. A chamada ao broker ocorre fora de qualquer transacao ou `DbContext` de longa vida. Se a publicacao falhar, o lote para naquela mensagem e ela permanece elegivel para nova tentativa.

Na primeira versao, `BatchSize = 1` deve ser o padrao. Um lote configuravel pode reduzir round trips quando o transporte estiver saudavel, desde que o acknowledgement continue individual e o processamento pare na primeira falha.

## Configuracao minima

| Opcao | Uso |
| --- | --- |
| `DispatcherName` | identidade estavel da lease |
| `ConsumerName` | linha de consumo atendida |
| `PollInterval` | espera sem trabalho ou sem lideranca |
| `BatchSize` | limite de leitura por ciclo |
| `LeaseDuration` | duracao da lideranca |
| `RenewBefore` | antecedencia para renovacao |

Um `DispatcherName` deve mapear para um unico `ConsumerName`. Dois workers para o mesmo consumidor precisam compartilhar o mesmo nome de dispatcher e, portanto, a mesma lease.

## Semantica

O worker publica antes de confirmar. Uma falha entre essas operacoes pode duplicar a entrega, mas nao perde a mensagem. Ele nao promete ordem global entre transacoes concorrentes; a ordem aplicada e a das mensagens visiveis retornadas pelo leitor confiavel de [an-consumer-cursor-commit.md](an-consumer-cursor-commit.md).

## Decisao

Introduzir um worker generico pequeno, com escopos curtos, lote unitario inicial e dependencias por interface. Lideranca, retries e metricas entram como colaboradores do worker, nao como logica do adaptador RabbitMQ/Kafka.