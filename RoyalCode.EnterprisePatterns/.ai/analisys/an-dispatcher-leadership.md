# Lideranca de dispatcher e fencing

## Objetivo

Permitir varias instancias da aplicacao, mas somente uma instancia ativa por `DispatcherName`, sem confiar no relogio local e sem deixar uma instancia que perdeu a lideranca confirmar consumo.

## Modelo persistido

Criar `outbox_dispatcher_leases` com uma linha por dispatcher:

| Coluna | Regra |
| --- | --- |
| `dispatcher_name` | chave primaria e nome estavel |
| `owner_instance_id` | Guid novo por processo iniciado |
| `lease_until` | instante UTC calculado pelo banco |
| `fencing_token` | inteiro crescente de geracao |

## Contrato

O nucleo expoe `IOutboxDispatcherLeaseCoordinator` com `TryAcquireAsync`, `TryRenewAsync` e `ReleaseAsync`. A aquisicao bem-sucedida devolve um `LeaseHandle` imutavel com dispatcher, owner e fencing token.

O acknowledger recebe esse handle e confirma uma mensagem somente se a mesma linha de lease ainda tiver owner, token e prazo validos. Essa verificacao precisa estar no mesmo `INSERT`/transacao da confirmacao; consultar a lease antes e confirmar depois abre uma janela de corrida.

## Implementacao PostgreSQL

Usar uma instrucao atomica `INSERT ... ON CONFLICT ... DO UPDATE` ou SQL equivalente. O banco usa `CURRENT_TIMESTAMP`, nao `DateTime.UtcNow` da aplicacao, para evitar skew de relogio.

- Sem linha: criar token `1`.
- Mesmo owner com lease valida: renovar sem trocar token.
- Lease expirada: adquirir com novo token, inclusive quando o processo antigo tem o mesmo owner, para separar geracoes apos uma pausa longa.
- Outro owner com lease valida: devolver que a lideranca nao foi adquirida.

Nenhuma conexao ou lock fica aberto durante a publicacao no broker. A renovacao ocorre por periodo, por exemplo lease de 30 segundos renovada aos 10 segundos, e nao por mensagem.

## Alternativa: advisory lock

Um advisory lock PostgreSQL e rapido, mas depende de uma conexao viva, nao sobrevive bem a pools/reconexoes e nao fornece fencing token para proteger o commit. Nao e recomendado para o caminho confiavel.

## Decisao

Usar lease persistida curta e fencing token. O custo e uma linha por dispatcher e uma operacao de renovacao periodica; em troca, o worker pode escalar horizontalmente sem avancar acknowledgements depois de perder a lideranca.