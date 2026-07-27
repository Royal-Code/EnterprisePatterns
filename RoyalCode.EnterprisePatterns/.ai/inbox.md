# Inbox

## Visão geral

`RoyalCode.Inbox` implementa a parte consumidora do padrão **Inbox**: ela recebe eventos que podem ser entregues mais de uma vez, aplica o efeito de negócio uma única vez por `eventId`, registra falhas de negócio de forma persistida e promove mensagens que excederam o limite de tentativas para uma caixa de mensagens venenosas (*poison messages*).

O módulo é usado em conjunto com o Outbox, mas sua responsabilidade é distinta:

- **Outbox** torna confiável a intenção de publicar uma alteração local;
- **Inbox** torna idempotente o processamento de uma entrega recebida e oferece tratamento operacional para falhas de negócio;
- **Inbox.RabbitMQ** é a borda que valida, abre e encaminha envelopes `OutboxTransportEnvelope<TEvent>` ao pipeline interno.

O Inbox não cria uma transação distribuída entre o consumidor, o banco local, RabbitMQ e sistemas externos. A garantia é de **processamento idempotente pelo identificador do evento**, desde que o efeito de negócio também seja idempotente quando envolver recursos externos.

## Componentes

| Componente | Responsabilidade |
| --- | --- |
| `RoyalCode.Inbox` | Núcleo: deduplicação, registro de problemas, poison, replay, contratos, opções e mapeamentos EF Core. |
| `RoyalCode.Inbox.RabbitMQ` | Consome envelopes do RabbitMQ, valida metadados, abre payloads protegidos e despacha o evento tipado. |
| `MessageObserverBase<TMessage, TDbContext>` | Classe-base para observers idempotentes que executam o caso de uso, confirmam sucesso e aplicam a política de retry/poison. |
| `InboxService` | Fachada usada pelos observers para registrar consumo e falhas de negócio. |
| `InboxPoisonReplayStorage<TDbContext>` | Executa replay manual protegido e transacional, limpando o estado do Inbox e criando uma nova mensagem na Outbox. |
| `InboxReplayApi` | API administrativa existente no `UserAccounts.Api` para replay autorizado e auditável. |

## Design

### Fluxo de consumo bem-sucedido

```text
RabbitMQ / ponte de federação
  │ entrega OutboxTransportEnvelope<TEvent>
  ▼
RabbitMQInboxTransportConsumer<TEvent>
  │ valida tipo, versão, política e payload
  │ abre ciphertext quando necessário
  ▼
IMessageDispatcher
  ▼
MessageObserverBase<TMessage, TDbContext>
  │ InboxService.TryConsumeEvent(event.Id)
  │  ├─ já consumido: encerra sem aplicar efeito
  │  └─ novo: adiciona InboxConsumedMessage ao DbContext
  ▼
ConsumeAsync(evento) retorna Result
  │
  └─ sucesso: SaveChangesAsync confirma InboxConsumedMessage + efeito local
```

A deduplicação usa o `Guid` do evento (`IHasId.Id`), e não o identificador de entrega do broker nem `OutboxTransportEnvelope.SourceMessageId`. Uma segunda entrega do mesmo evento encontra `InboxConsumedMessage` e não executa o observer novamente.

O marcador de consumo é adicionado ao **mesmo `DbContext`** do observer e só é gravado após o retorno de sucesso de `ConsumeAsync`. Assim, o efeito local e a confirmação de que o evento foi consumido participam do mesmo `SaveChangesAsync`, desde que o observer altere o mesmo contexto.

### Fluxo de falha de negócio, retry e poison

```text
MessageObserverBase.ConsumeAsync(evento)
  │ retorna Result com Problems
  ▼
InboxService.LogProblems(eventId, evento, problems)
  │ cria novo escopo e persiste diagnóstico em transação separada
  ▼
InboxMessageProblems
  │ Occurrences ainda dentro da política
  ├─ sim: retorna true; observer lança InvalidOperationException
  │       e o transporte pode redeliver a mensagem
  │
  └─ não: serializa/protege o evento, cria InboxPoisonMessage,
          cria InboxConsumedMessage e retorna false
          O observer encerra sem nova exceção.
```

A separação de transações é deliberada: a ocorrência do problema é persistida mesmo que a transação do observer seja descartada para permitir redelivery. Enquanto a mensagem ainda pode ser reprocessada, `MessageObserverBase` lança `InvalidOperationException` após registrar o problema. Quando é promovida a poison, ela é marcada como consumida e o observer retorna normalmente, evitando retentativas infinitas pelo mesmo pipeline.

`InboxOptions.ReprocessingAttempts` representa as **retentativas após a primeira falha**. Com o valor padrão `3`:

1. a primeira falha cria `InboxMessageProblems` com `Occurrences = 1` e provoca redelivery;
2. a segunda e a terceira falhas seguem provocando redelivery;
3. na quarta falha, `Occurrences` passa a `4`, excede o limite, a mensagem é movida para poison e marcada como consumida.

O primeiro registro de problema sempre permite uma nova tentativa. Por isso, mesmo `ReprocessingAttempts = 0` resulta em uma redelivery antes da promoção para poison na falha seguinte.

### Fluxo de replay manual

```text
Operador autorizado
  │ POST /api/admin/inbox/poison/{eventId}/replay
  ▼
IReplayInboxPoisonHandler
  │ valida eventId, operador e motivo
  ▼
InboxPoisonReplayStorage<TDbContext>
  │ valida contrato simultaneamente configurado no Inbox e Outbox
  │ converte payload plain/legado elegível para protegido
  ▼
Mesma transação EF Core
  ├─ remove InboxPoisonMessage
  ├─ remove InboxConsumedMessage
  ├─ remove InboxMessageProblems
  ├─ cria OutboxMessage protegida
  └─ cria InboxPoisonReplayAudit com motivo protegido
  ▼
Dispatcher da Outbox publica novamente
  ▼
Inbox recebe o mesmo evento e pode aplicar seu efeito novamente
```

A nova `OutboxMessage` possui outro Id sequencial, mas o payload preserva o identificador original do evento. Ao remover `InboxConsumedMessage`, o replay permite que o mesmo `eventId` volte a ser processado. A entrada de auditoria armazena operador, contrato, versão, data e motivo cifrado; ela não duplica o payload do evento.

## Modelos persistidos

As configurações ficam em `EntityFramework/Mappings/InboxConfigurations.cs`. Nos contextos que aplicam `ApplyLowerCaseConvention()`, como `UserAccountsDbContext` e `KeyManagerDbContext`, os nomes físicos ficam em minúsculas.

| Tabela lógica | Conteúdo | Regras |
| --- | --- | --- |
| `InboxConsumedMessage` | `Id` do evento e `ConsumedAt`. | `Id` é chave primária e também chave de idempotência. |
| `InboxMessageProblems` | `Id`, `Occurrences` e coleção serializada de `InboxProblemOccurrence`. | A coleção de Problems é JSON em coluna `text`, com `ValueConverter` e `ValueComparer` próprios. |
| `InboxPoisonMessage` | Id do evento, data, tipo, versão, proteção, chave e payload. | O payload é `text`; o tipo tem até 100 caracteres; a metadata de proteção pode ser nula em registros legados. |
| `InboxPoisonReplayAudit` | Id do replay, Id do evento, contrato, versão, operador, motivo protegido, chave e data. | Possui índice por `EventId`; `OperatorId` tem até 200 e `ProtectionKeyId` até 100 caracteres. |

O tipo `InboxMessage` existe no projeto, mas não é mapeado por `ApplyInboxConfigurations()` e não participa do fluxo atual de consumo, problemas ou replay. Não o use como uma fila persistida nova sem antes definir seu mapeamento e ciclo de vida.

Não há expurgo automático de marcadores consumidos, diagnósticos, poison ou auditorias. A retenção precisa ser definida conforme o período de redelivery esperado, exigências de auditoria e a possibilidade de replay manual.

## Funcionalidades e regras

### Deduplicação

`InboxService.TryConsumeEvent(eventId, ct)` delega a `IConsumeMessageHandler`:

1. consulta `InboxConsumedMessage` por Id;
2. se o evento já foi consumido, retorna `false` ao observer;
3. se não foi, adiciona `InboxConsumedMessage` ao contexto, mas não salva;
4. o observer só confirma a inserção depois de executar o caso de uso com sucesso.

A chave primária de `InboxConsumedMessage` protege a identidade do evento no banco. Porém, o handler faz primeiro uma consulta e depois adiciona a entidade; duas entregas concorrentes podem passar pela consulta antes de qualquer uma confirmar. Uma delas deverá falhar no `SaveChanges` por chave duplicada. Consumers e infraestrutura devem estar preparados para esse cenário de concorrência e não considerar a deduplicação uma transação distribuída.

### Observers baseados em `Result`

`MessageObserverBase<TMessage, TDbContext>` exige que a mensagem implemente `IHasId` e que o processamento retorne `Result`:

```csharp
public sealed class PedidoAprovadoObserver
    : MessageObserverBase<PedidoAprovado, PedidosDbContext>
{
    private readonly ProcessarPedidoAprovadoHandler handler;

    public PedidoAprovadoObserver(
        InboxService inbox,
        PedidosDbContext db,
        ProcessarPedidoAprovadoHandler handler)
        : base(inbox, db)
    {
        this.handler = handler;
    }

    protected override Task<Result> ConsumeAsync(
        PedidoAprovado message,
        CancellationToken ct)
        => handler.HandleAsync(message, ct);
}
```

Regra importante: erros de negócio esperados devem ser devolvidos como `Result`/`Problems`, não como exceções. Só esse caminho registra tentativas, cria poison quando necessário e decide se deve haver redelivery.

Exceções técnicas lançadas por `ConsumeAsync` não passam por `InboxService.LogProblems`; elas sobem para o transporte. O comportamento de redelivery, log e dead letter, nesse caso, depende do consumidor e da configuração do broker.

Registre observers no dispatcher interno:

```csharp
services.AddMessageObserver<PedidoAprovadoObserver, PedidoAprovado>();
```

### Registro de problemas e poison

Ao receber `Result` com Problems, `LogMessageProblemsHandler<TDbContext>` cria um **novo escopo de DI** e usa outro `DbContext` para persistir o diagnóstico independentemente da tentativa em andamento.

Enquanto `Occurrences <= ReprocessingAttempts`, o método retorna `true`; o observer lança e o transporte pode redeliver. Quando excede o limite, o handler:

1. busca o metadata do tipo no `InboxOptions`;
2. serializa a mensagem com as opções do tipo ou opções padrão;
3. resolve `PayloadProtection` e, se necessário, cifra o JSON;
4. persiste `InboxPoisonMessage`;
5. persiste `InboxConsumedMessage` para impedir novo processamento automático;
6. retorna `false` para encerrar a entrega normalmente.

Se o tipo não estiver configurado, o handler cria metadata de contingência com nome `Auto:<NomeDoTipo>` e versão `1` para não deixar a mensagem em loop infinito. Essa mensagem poison poderá ser investigada, mas não será elegível para replay até que um contrato correspondente seja configurado tanto no Inbox quanto no Outbox.

`InboxMessageProblems` não é removida ao promover para poison. Ela é preservada como histórico de diagnósticos e só é removida pelo replay bem-sucedido.

### Proteção de payload

`InboxOptions` herda `BoxOptionsBase<InboxOptions>`:

- a política padrão do núcleo é `PayloadProtectionMode.Plain`;
- cada tipo pode herdar a política ou escolher `Plain`/`Protected`;
- a serialização padrão é JSON compacto com propriedades `camelCase`;
- `LegacyPlainPayloadReadUntilUtc` permite reconhecer payloads sem metadata de proteção apenas antes da data de corte.

Para criar poison ou efetuar replay de dados protegidos, configure o serviço de proteção antes de registrar o Inbox:

```csharp
// Configure as chaves por meio da infraestrutura segura do ambiente.
services.AddValueProtection(/* configuração corporativa de chaves */);
services.AddProtectedMessageCodec();
```

Com payload `Protected`, o poison armazena ciphertext e `ProtectionKeyId`. O replay mantém o ciphertext se a poison já estiver protegida; se ela estiver em formato `Plain` ou legado elegível, ela é cifrada antes de criar a nova mensagem na Outbox. O motivo administrativo do replay é sempre protegido.

### Replay de poison

O contrato de replay é:

```csharp
new ReplayInboxPoison(
    EventId: eventId,
    OperatorId: "operador-autenticado",
    Reason: "Motivo operacional do reprocessamento");
```

`ReplayInboxPoisonHandler` valida:

- `EventId` diferente de `Guid.Empty`;
- operador obrigatório e com até 200 caracteres;
- motivo obrigatório e com até 500 caracteres.

O storage devolve `NotFound` quando não há poison com o evento informado e `InvalidState` quando:

- tipo/versão não estão configurados simultaneamente no Inbox e Outbox;
- payload protegido não possui `ProtectionKeyId`;
- o payload plain/legado não é elegível para proteção e replay;
- há conflito de concorrência durante a alteração do estado.

O replay não publica diretamente no RabbitMQ. Ele agenda uma nova `OutboxMessage` na mesma transação; o dispatcher da Outbox é responsável por enviá-la segundo a política normal do deployável.

## Integração com Entity Framework

O Inbox deve compartilhar o `DbContext` do módulo que aplica o efeito do evento. Um contexto típico que usa Inbox e Outbox contém ambos os mapeamentos:

```csharp
public sealed class PedidosDbContext : DbContext
{
    public PedidosDbContext(DbContextOptions<PedidosDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyPedidosConfigurations();
        modelBuilder.ApplyInboxConfigurations();
        modelBuilder.ApplyOutboxConfigurations(); // necessário se houver replay
        modelBuilder.IgnoreHasEvents();
        modelBuilder.ApplyLowerCaseConvention();
    }
}
```

Não crie um `DbContext` separado somente para marcar o Inbox como consumido. Isso separa o marcador de idempotência do efeito de negócio e pode gerar eventos marcados como consumidos sem que sua alteração local tenha sido confirmada.

A alteração do schema segue o fluxo oficial de migrations do banco do módulo: gerar migration com `dotnet ef`, gerar script SQL idempotente na pasta `sql/` do projeto de migrations e entregar o artefato ao DBA. A aplicação não executa migrations em runtime.

## Configuração e uso

### Registro do núcleo

Para um módulo que trata eventos e precisa de replay, configure Inbox e Outbox para o mesmo contexto e os mesmos contratos elegíveis:

```csharp
services.AddDbContext<PedidosDbContext>((sp, options) =>
    options.UseNpgsql(sp.GetConnectionString("Pedidos")));

services.AddValueProtection(/* chaves corporativas */);
services.AddProtectedMessageCodec();

services.AddInboxServices<PedidosDbContext>();
services.ConfigureInbox(options =>
{
    options.DefaultPayloadProtection = PayloadProtectionMode.Protected;
    options.ReprocessingAttempts = 3;
    options.AddMessageType<PedidoAprovado>("order-approved");
});

services.AddOutboxServices<PedidosDbContext>();
services.ConfigureOutbox(options =>
{
    options.DefaultPayloadProtection = PayloadProtectionMode.Protected;
    options.AddMessageType<PedidoAprovado>("order-approved");
});

services.AddMessageObserver<PedidoAprovadoObserver, PedidoAprovado>();
```

Para consumo sem replay manual, o registro de Outbox não é necessário ao mecanismo de deduplicação. Para replay, ele é obrigatório: `InboxPoisonReplayStorage` exige que o par `(MessageType, VersionType)` esteja configurado nas duas caixas e insere uma `OutboxMessage`.

### Consumo RabbitMQ

`AddInboxRabbitMQ` registra o catálogo de transporte, o `IProvider<T>` que cria escopos por execução, a mensageria RabbitMQ e o health check. O tipo de mensagem do canal é sempre `OutboxTransportEnvelope<TEvent>`:

```csharp
services.AddInboxRabbitMQ(
    transport => transport.AddMessageType<PedidoAprovado>("order-approved", versionType: 1),
    rabbit =>
    {
        rabbit.ConfigureConnection(options =>
            options.UseConnectionStringFromConfiguration("RabbitMQ"));

        rabbit.AddChannel<OutboxTransportEnvelope<PedidoAprovado>>(
                ChannelInfo.Topic(
                    exchange: "RoyalCode.pedidos",
                    topic: "order-approved.v1",
                    queueName: "faturamento.order-approved",
                    activateDeadLetter: true))
            .UseForReceiveOnly()
            .UseInboxTransportConsumer<PedidoAprovado>()
            .UseExclusiveStrategyForReceive(options => options.PrefetchCount = 5);
    });
```

A configuração precisa conter três catálogos coerentes:

1. `InboxOptions.AddMessageType<TEvent>` para desserializar e persistir poison;
2. `OutboxTransportOptions.AddMessageType<TEvent>` para validar o envelope recebido;
3. um canal `OutboxTransportEnvelope<TEvent>` configurado para recepção.

`RabbitMQInboxTransportConsumer<TEvent>` executa as etapas abaixo:

1. verifica se há envelope;
2. valida `PayloadProtection`, `ProtectionKeyId` e `Payload`;
3. confere se `(MessageType, VersionType)` corresponde ao tipo genérico e ao catálogo de transporte;
4. confirma se o tipo também está habilitado no `InboxOptions`;
5. abre o ciphertext quando necessário;
6. desserializa `TEvent` e chama `IMessageDispatcher`.

Erros técnicos de envelope — metadata inválida, contrato inesperado, JSON inválido, chave ausente ou ciphertext corrompido — fazem o consumer chamar `incomingMessage.Reject()`. Para que isso tenha destino operacional, a fila precisa de dead letter configurado no RabbitMQ.

Erros de negócio retornados pelos observers não são rejeitados pelo transport consumer: eles seguem a política de `MessageObserverBase` e podem propagar uma exceção para que a infraestrutura de recepção aplique redelivery. Não combine um consumer técnico próprio, `AutoDispatch` e `UseInboxTransportConsumer` no mesmo canal, pois são estratégias diferentes e podem duplicar o processamento.

### Configuração de referência: UserAccounts

O `UserAccounts.Api` usa uma composição única de RabbitMQ para publicar sua Outbox e receber `EscopoAprovado`. A configuração de produção contém:

```json
{
  "Inbox": {
    "QueueName": "iam.useraccounts.scope-approved.duende",
    "PrefetchCount": 1,
    "ReprocessingAttempts": 3
  },
  "OutboxRabbitMQ": {
    "Routes": {
      "scope-approved": {
        "Exchange": "RoyalCode.identidade.escopo",
        "RoutingKey": "scope-approved.v1"
      }
    }
  }
}
```

No startup, `UserAccountsInboxOptions` exige fila não vazia, `PrefetchCount > 0` e tentativas não negativas. O módulo configura `EscopoAprovado` como contrato do Inbox com payload protegido, associa a fila ao tópico `scope-approved.v1`, habilita dead letter e usa `PrefetchCount` na estratégia exclusiva de recepção.

O consumer `EscopoAprovadoConsumer` sincroniza o Identity Resource do Duende. Como essa operação toca outro store, ela precisa ser um *upsert* idempotente: se o efeito externo ocorrer e o commit local do Inbox falhar, a redelivery repetirá o upsert antes de confirmar `InboxConsumedMessage`.

### API administrativa de replay

O núcleo não expõe endpoints HTTP. No `UserAccounts.Api`, o replay é publicado explicitamente em:

```text
POST /api/admin/inbox/poison/{eventId}/replay
```

Corpo:

```json
{
  "reason": "Falha operacional corrigida no sistema externo"
}
```

A rota exige `PolicyConstants.AppOnlyPolicy` e a funcionalidade `FN_USRINBREP`. O operador é extraído do claim `sub` ou, para `app_credentials`, do claim de aplicativo. O teste de integração confirma `401 Unauthorized` para chamada anônima e valida que uma chamada autorizada cria auditoria com o motivo protegido.

## Casos de uso observados

### Sincronização de Identity Scope para Duende

`EscopoAprovadoConsumer` consome `EscopoAprovado`, deduplica pelo Id do evento e chama `ISincronizadorIdentityResource`. Falhas de negócio retornadas pelo sincronizador acionam retry; após esgotamento, a mensagem é protegida, movida para poison e marcada como consumida. O replay manual a reinsere na Outbox e permite nova sincronização.

Os testes de integração com PostgreSQL real validam:

- duas entregas do mesmo envelope aplicam um único efeito;
- uma falha transitória não marca a mensagem como consumida e gera `InboxMessageProblems`;
- depois do limite, a poison é protegida e não bloqueia o próximo evento;
- replay remove poison, consumo e problemas, cria uma `OutboxMessage` protegida e audita o motivo;
- envelope com chave indisponível é rejeitado sem aplicar efeito;
- o cenário RabbitMQ é coberto quando o broker de integração está disponível.

### Replicação de KMS/federação

No `KeyManager`, observers de tokens, chaves e adesão à federação herdam `KmsObserverBase<TMessage>`, que é baseado em `MessageObserverBase`. A ponte `OutboxConsumeHandler` busca lotes na API HTTP do vault master, despacha-os localmente pelo `OutboxService` e confirma o cursor remoto somente após concluir o lote. Assim, a réplica usa Inbox para não reaplicar eventos durante redelivery ou nova leitura do mesmo lote.

## Formas incorretas de trabalhar

| Prática incorreta | Consequência | Forma correta |
| --- | --- | --- |
| Marcar `InboxConsumedMessage` e chamar `SaveChanges` antes de executar o caso de uso. | Uma falha posterior descarta a entrega sem aplicar o efeito. | Deixar `MessageObserverBase` salvar o marcador junto do sucesso do observer. |
| Criar um DbContext separado para o marcador de consumo. | A deduplicação deixa de ser atômica com a alteração local. | Usar o mesmo contexto do caso de uso no observer. |
| Lançar exceção para erro de negócio esperado. | Não há registro de Problems, contador de tentativas nem poison pelo Inbox. | Retornar `Result` com `Problems` no `ConsumeAsync`. |
| Retornar sucesso depois de detectar uma regra de negócio inválida. | O evento é marcado como consumido e não terá retry nem poison. | Retornar `Problems` e deixar a política do Inbox decidir. |
| Configurar payload `Plain` para evento ou poison sensível. | Conteúdo fica exposto no banco e pode atravessar o broker em claro. | Usar `PayloadProtectionMode.Protected`, `ProtectionService` e `AddProtectedMessageCodec`. |
| Configurar somente o canal RabbitMQ e omitir os catálogos Inbox/transporte. | O envelope é rejeitado ou a poison não consegue serializar o contrato corretamente. | Registrar o tipo em `InboxOptions`, `OutboxTransportOptions` e no canal tipado. |
| Compartilhar uma fila entre assinantes independentes. | Os consumidores competem pelas mensagens e alguns deixam de receber eventos. | Criar fila nomeada própria por assinatura. |
| Não habilitar dead letter para a fila que rejeita envelope técnico. | Mensagens inválidas podem ser descartadas sem trilha operacional adequada. | Ativar dead letter e monitorar a fila correspondente. |
| Reproduzir poison diretamente no RabbitMQ. | Perde a atomicidade entre limpeza do Inbox, auditoria e agendamento. | Usar `IReplayInboxPoisonHandler`, que grava a nova entrega na Outbox. |
| Reproduzir poison sem configurar o mesmo contrato no Inbox e Outbox. | O replay retorna `InvalidState`; não há garantia de que o receptor entenderá a nova entrega. | Manter tipo e versão compatíveis nas duas caixas. |
| Expor replay sem política de autorização e identificação do operador. | Uma operação destrutiva e sensível fica acessível sem auditoria suficiente. | Usar política de aplicação, funcionalidade e motivo obrigatório, como no `InboxReplayApi`. |

## Limitações e riscos atuais

1. **Idempotência baseada no Id do evento.** Eventos sem `IHasId` não podem usar `MessageObserverBase`; eventos logicamente distintos com o mesmo Id serão descartados como duplicados.
2. **Corrida na deduplicação.** O fluxo consulta e depois insere o marcador; concorrência real pode produzir `DbUpdateException` por chave duplicada. Não há tratamento específico dessa exceção no núcleo.
3. **Somente Problems entram na política de retry.** Exceções técnicas de observers não geram `InboxMessageProblems` nem poison automaticamente.
4. **Política de redelivery depende do transporte.** `RabbitMQInboxTransportConsumer` rejeita falhas técnicas, mas a redelivery de exceções de negócio depende da configuração/execução do receiver RabbitMQ.
5. **Poison não é uma DLQ do broker.** Problemas de negócio persistem em `InboxPoisonMessage`; envelopes tecnicamente inválidos são rejeitados no broker e podem seguir para dead letter sem registro no Inbox.
6. **Sem consulta operacional genérica.** O núcleo fornece replay por Id, mas não possui API genérica para listar, filtrar ou inspecionar consumidos, problemas e poison.
7. **Sem retenção automática.** Tabelas de consumo e problemas crescem continuamente; expurgo deve preservar a janela de redelivery e necessidades de auditoria.
8. **Problems serializados como texto.** A coleção de ocorrências não é facilmente filtrável ou agregável por SQL sem processamento adicional.
9. **Fallback `Auto:<Tipo>`.** Ele evita loop infinito ao criar poison de tipo não configurado, mas impede replay até que o contrato seja formalizado nas duas caixas.
10. **Proteção padrão é Plain.** Sem configuração explícita, uma poison pode conter JSON em claro.
11. **Replay é acoplado ao Outbox.** Uma aplicação que precisa replay deve possuir os mapeamentos, opções e dispatcher da Outbox; o Inbox sozinho não publica mensagens.
12. **Cobertura do núcleo usa InMemory em parte dos testes.** A concorrência, isolamento e comportamento de PostgreSQL precisam continuar cobertos por testes de integração reais, como os existentes no UserAccounts.

## Observabilidade e operação

O núcleo do Inbox não define métricas OpenTelemetry, health check próprio, contador de tentativas exposto ou logs estruturados centralizados. O adaptador `AddInboxRabbitMQ` registra o health check do RabbitMQ; o deployável deve expor e monitorar esse check junto do banco do módulo.

Métricas e alertas recomendados:

- quantidade e idade de `InboxPoisonMessage`;
- crescimento de `InboxMessageProblems` e distribuição de `Occurrences`;
- taxa de `Reject()`/dead letter no RabbitMQ;
- falhas de abertura de payload por chave indisponível ou ciphertext inválido;
- tempo entre a criação da poison e seu replay;
- conflitos de chave em `InboxConsumedMessage` sob carga concorrente;
- falhas de sincronização externa que retornam `Problems`.

Como o payload pode ser protegido, não registre ciphertext, JSON de evento, motivo de replay ou dados sensíveis em logs de aplicação. Registre identificadores, tipo, versão, `SourceMessageId` quando disponível e correlação/causação da operação.

## Testes existentes

| Área | Evidências |
| --- | --- |
| Observer e deduplicação básica | `RoyalCode.Inbox.Tests/InboxTests.cs` valida consumo e registro de Problems. |
| Poison protegido | O mesmo teste valida que o payload não é gravado em claro quando o tipo usa `Protected`. |
| Envelope RabbitMQ | `RabbitMQInboxTransportConsumerTests` cobre payload plain/protegido e rejeição de envelope inválido. |
| Redelivery e poison no broker | `RabbitMQInboxTransientFailureIntegrationTests`, `RabbitMQInboxPoisonIntegrationTests` e `RabbitMQReceiverRedeliveryIntegrationTests`. |
| UserAccounts com PostgreSQL | `InboxIdentityScopeDuendeSyncIntegrationTests` cobre duplicata, retry, poison, replay, rejeição por chave inexistente e integração RabbitMQ. |
| API administrativa | `InboxReplayApiIntegrationTests` cobre acesso anônimo e replay autorizado com auditoria protegida. |

Os testes de integração RabbitMQ exigem broker disponível e, em alguns casos, a variável `USERACCOUNTS_TEST_RABBITMQ_PASSWORD`. O projeto de testes unitários do Inbox ainda usa `UseInMemoryDatabase`; ele não substitui a validação contra PostgreSQL real para comportamento concorrente.

## Oportunidades de melhoria

1. Tratar explicitamente violação de chave única em `InboxConsumedMessage` como entrega duplicada, evitando erro técnico em corrida concorrente.
2. Diferenciar e persistir falhas técnicas transitórias de falhas de negócio, com política de retry e observabilidade próprias.
3. Criar consultas e endpoints administrativos paginados para poison, Problems e auditoria, protegidos por funcionalidade corporativa.
4. Implementar retenção configurável para consumidos, problemas e auditorias, respeitando janelas de redelivery e compliance.
5. Persistir dados de origem do transporte — broker, exchange, routing key, `SourceMessageId`, correlação e causação — para facilitar diagnóstico.
6. Adicionar métricas, logs estruturados e health checks de Inbox genéricos no projeto núcleo.
7. Tornar explícita a semântica de `ReprocessingAttempts` no tipo de opções ou substituir por nomes menos ambíguos, como `RetriesAfterFirstFailure`.
8. Registrar metadata formal do contrato antes de criar poison, em vez de depender do fallback `Auto:<Tipo>` sem caminho de replay.
9. Criar testes PostgreSQL de concorrência para duas entregas simultâneas do mesmo `eventId`.
10. Definir política de versionamento de eventos para o caminho Inbox/Outbox/Transporte, pois o registro de alto nível ainda cria versão `1`.
11. Avaliar uma abstração comum de composição RabbitMQ para evitar que deployáveis precisem combinar manualmente Outbox e Inbox na mesma configuração de canais.

## Arquivos de referência

- `src/RoyalCode.Inbox/Services/MessageObserverBase.cs`
- `src/RoyalCode.Inbox/Services/InboxService.cs`
- `src/RoyalCode.Inbox/Services/Internals/ConsumeMessageHandler.cs`
- `src/RoyalCode.Inbox/Services/Internals/LogMessageProblemsHandler.cs`
- `src/RoyalCode.Inbox/EntityFramework/InboxPoisonReplayStorage.cs`
- `src/RoyalCode.Inbox/EntityFramework/Mappings/InboxConfigurations.cs`
- `src/RoyalCode.Inbox.RabbitMQ/Services/RabbitMQInboxTransportConsumer.cs`
- `src/RoyalCode.Inbox.RabbitMQ/Extensions/InboxRabbitMQServiceCollectionExtensions.cs`
- `src/RoyalCode.UserAccounts.Api/Apis/InboxReplayApi.cs`
- `src/RoyalCode.UserAccounts/Integrations/Duende/EscopoAprovadoConsumer.cs`
- `src/RoyalCode.KeyManager/Integrations/OutboxConsumeHandler.cs`
- `src/RoyalCode.Inbox.Tests/InboxTests.cs`
- `src/RoyalCode.UserAccounts.Tests/Integration/InboxIdentityScopeDuendeSyncIntegrationTests.cs`

Também foi consultada a referência interna `.ai/references/messaging/RoyalCode-messaging-rabbitmq-usage.md`.
