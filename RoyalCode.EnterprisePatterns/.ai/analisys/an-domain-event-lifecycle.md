# Ciclo de vida de eventos de dominio

## Reavaliacao

O comportamento apontado nao e um defeito no caminho normal de uso. Com um `DbContext` por unidade de trabalho, o `OutboxTracker` observa o agregado uma vez, acumula os eventos em sua lista temporaria e a limpa depois de adicionar as `OutboxMessage` ao mesmo contexto. Um segundo `SaveChanges` no mesmo contexto nao reinsere os mesmos eventos.

O risco aparece quando a mesma instancia de agregado, ja salva, e anexada a outro `DbContext`: `DomainEventCollection.Observe` entrega tambem os eventos que ja estavam na colecao, e o novo tracker os grava novamente. Esse fluxo nao deve ser normal em uma aplicacao que respeita a fronteira da unidade de trabalho, mas e possivel em jobs, testes, caches de objetos e codigo que reaproveita entidades destacadas.

## Comportamento atual

```text
agregado gera evento
  -> tracker observa e coloca o evento em lista temporaria
  -> SavingChanges cria OutboxMessage no mesmo DbContext
  -> lista temporaria do tracker e limpa
  -> DomainEvents do agregado permanece com o evento
```

Manter o evento na origem tambem tem uma vantagem em falhas: se o contexto for descartado antes de confirmar a transacao, uma nova unidade de trabalho ainda pode observar o evento e gravar a intencao novamente. Em caso de resultado de commit desconhecido, uma duplicata tambem e uma consequencia esperada da entrega pelo menos uma vez; o `EventId` futuro sera a chave de idempotencia do Inbox.

## Contrato recomendado agora

Documentar a regra abaixo junto de `OutboxTracker.Initialize` e `IHasEvents`:

> `DomainEvents` representa eventos pendentes da unidade de trabalho. Depois de um `SaveChanges` bem-sucedido, nao reutilize nem reanexe a mesma instancia de agregado em outro `DbContext`. Recarregue o agregado pelo novo contexto.

Isso e simples, consistente com o uso normal de EF Core e suficiente para a fase atual. Nao deve bloquear os demais trabalhos do Outbox.

## Evolucao opcional

Quando houver necessidade real de reutilizar agregados entre contextos, o tracker deve limpar apenas os eventos que foram confirmados, e somente apos `SavedChanges`:

1. Guardar pares `(colecao, evento)` ao observar os eventos.
2. No `SavingChanges`, marcar o lote como gravado no contexto, sem remove-lo da colecao.
3. Em `SavedChanges`, remover somente os eventos daquele lote das colecoes de origem.
4. Em falha, manter o lote para que a mesma intencao possa ser recuperada.
5. Evitar criar uma segunda `OutboxMessage` quando o mesmo `DbContext` repetir o save apos falha, pois a primeira mensagem ainda esta rastreada pelo EF Core.

Essa evolucao exige testes de sucesso, falha, retry no mesmo contexto e reanexo em novo contexto. Nao e seguro simplesmente chamar `DomainEvents.Clear()` em `SavingChanges`: isso perde eventos quando a transacao falha.

## Decisao

Tratar como contrato e documentacao agora. Registrar testes de comportamento para evitar regressao e postergar a limpeza pos-commit ate existir um caso de uso que justifique a complexidade adicional.