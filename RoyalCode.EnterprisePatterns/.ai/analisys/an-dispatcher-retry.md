# Retry de publicacao

## Objetivo

Repetir falhas temporarias de transporte sem confirmar a mensagem, sem criar estado persistido por tentativa antes que ele seja realmente necessario.

## Opcao A: retry no worker

O worker deixa a mensagem sem acknowledgement, encerra o lote na primeira falha e aguarda um atraso calculado antes de ler novamente. Uma politica `IOutboxRetryPolicy` recebe a contagem de falhas consecutivas e devolve o proximo atraso.

Configuracao minima: atraso inicial, atraso maximo, multiplicador exponencial e jitter. O tempo deve ser obtido por `TimeProvider` para tornar testes deterministas.

Essa opcao preserva a ordem das mensagens visiveis para aquele consumidor, usa zero colunas adicionais e mantem entrega pelo menos uma vez. Reiniciar o processo zera a contagem em memoria, o que e aceitavel na primeira versao: a mensagem continua pendente e nunca e descartada.

Falha de configuracao, como rota ausente, tambem bloqueia a confirmacao. Ela deve produzir log e health check degradado, em vez de pular a mensagem.

## Opcao B: estado persistido por consumidor e mensagem

Persistir `attempts`, `next_attempt_at` e `last_error` em uma tabela de entrega permite backoff sobreviver a reinicios, agendamento de poison e dashboards detalhados. Em troca, adiciona escrita para cada falha, politica de limpeza e semantica adicional para cada consumidor.

Essa opcao deve ser adotada apenas quando houver requisito operacional de limite de tentativas ou tratamento de mensagens venenosas no produtor. O Inbox continua sendo o lugar principal para erros de negocio do consumidor final.

## Decisao

Comecar pela opcao A. O primeiro worker processa uma mensagem por vez, para no primeiro erro, aplica backoff exponencial com jitter e nao confirma nada em caso de excecao. Evoluir para estado persistido somente com uma politica de poison e retencao definidas.