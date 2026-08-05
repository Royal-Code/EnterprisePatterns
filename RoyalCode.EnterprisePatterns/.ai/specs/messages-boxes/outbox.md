# Outbox

## Visão geral

`RoyalCode.Outbox` implementa o padrão **Transactional Outbox** para evitar a inconsistência entre uma alteração no banco de dados e a publicação de seu evento para outro processo. Em vez de o caso de uso publicar diretamente no RabbitMQ, ele grava uma `OutboxMessage` no mesmo `DbContext` e na mesma transação da mudança de domínio. Um dispatcher assíncrono lê essas mensagens posteriormente, publica-as em um transporte externo e avança um cursor persistido somente depois de considerar a publicação bem-sucedida.

O módulo é dividido em duas bibliotecas e uma implementação concreta de despacho:

| Componente | Responsabilidade |
| --- | --- |
| `RoyalCode.Outbox` | Núcleo independente de transporte: modelo EF Core, registro de consumidores, leitura por cursor, serialização, proteção de payload, tracker de eventos de domínio, lease e cursor cercado. |
| `RoyalCode.Outbox.RabbitMQ` | Adaptador que converte uma `OutboxMessage` em um `OutboxTransportEnvelope<TEvent>` e o publica no canal RabbitMQ tipado. |
| `RoyalCode.UserAccounts.Api` | Referência de produção: `OutboxRabbitMQDispatcherService`, um `BackgroundService` que obtém a lease, publica em ordem, confirma o cursor e produz métricas. |

O Outbox **não entrega exatamente uma vez**. A garantia prática é de **entrega ao menos uma vez**: uma mensagem pode ser reenviada se for publicada, mas o processo falhar antes da confirmação do cursor. Portanto, todos os consumidores devem ser idempotentes.

## Objetivos e limites do padrão

### O que resolve

1. A alteração de domínio e a intenção de publicar o evento são persistidas juntas no banco relacional.
2. Uma indisponibilidade do RabbitMQ não desfaz a mudança de negócio já confirmada.
3. A publicação pode ser retomada após reinício, por meio de mensagens e cursores persistidos.
4. Em múltiplas réplicas, uma lease persistida seleciona o líder do dispatcher para um nome estável.
5. O payload pode ser salvo e transportado protegido, preservando o ciphertext até o consumidor autorizado a abri-lo.

### O que não resolve sozinho

- Não há transação distribuída entre PostgreSQL e RabbitMQ.
- Não há deduplicação automática no consumidor nem garantia de exatamente uma vez.
- O núcleo não registra um `BackgroundService` genérico. Cada deployável deve fornecer seu dispatcher ou reutilizar a estratégia do `UserAccounts.Api`.
- Não há expurgo, arquivamento, retry contador por mensagem ou fila de mensagens venenosas do lado produtor.
- Não há autorização embutida em `MapOutbox`; o host que expõe a API HTTP deve protegê-la.

## Design

### Fluxo de escrita transacional

```text
Entidade de domínio (IHasEvents)
  │ adiciona evento a DomainEvents
  ▼
OutboxTracker, ligado ao DbContext
  │ evento SavingChanges
  ▼
OutboxService.Write(evento)
  │ serializa, resolve o tipo e aplica a proteção configurada
  ▼
OutboxMessage é adicionada ao mesmo DbContext
  │
  └── SaveChanges/SaveChangesAsync confirma alteração de domínio + OutboxMessage
      na mesma transação do banco
```

`OutboxTracker` é opt-in. Ele observa entidades que implementam `IHasEvents`, escuta eventos adicionados a `DomainEvents` e, no `SavingChanges`, chama `OutboxService.Write`. O serviço apenas adiciona a mensagem ao contexto; ele **não** chama `SaveChanges`. Essa escolha é essencial para que a entidade de negócio e a mensagem sejam confirmadas ou revertidas juntas.

Também é possível escrever explicitamente com `OutboxService.Write(evento)`, desde que o serviço e a alteração de domínio usem o mesmo `DbContext` e que o chamador confirme o contexto uma única vez.

### Fluxo de publicação com RabbitMQ

```text
OutboxRabbitMQDispatcherService (líder)
  │ obtém/renova lease por DispatcherName
  ▼
OutboxConsumer (cursor compartilhado)
  │ lê a próxima OutboxMessage por Id crescente
  ▼
IOutboxTransportPublisher.PublishAsync
  │
  ├── resolve (MessageType, VersionType)
  └── publica OutboxTransportEnvelope<TEvent> no canal RabbitMQ tipado
       │
       ▼
IOutboxDispatcherCursorCoordinator.TryCommitAsync
  │ exige lease válida, fencing token correto, Id existente e cursor crescente
  ▼
OutboxConsumer.LastConsumedMessageId avança
```

O dispatcher de referência lê uma mensagem por vez (`Limit = 1`) e preserva a ordem do `Id` para seu cursor. Ele só tenta confirmar o cursor depois de `PublishAsync` concluir sem exceção. A confirmação é protegida por lease e *fencing token*, impedindo que uma instância que perdeu a liderança avance o cursor.

Se a publicação falhar, o cursor não avança. Se a publicação ocorrer, mas o commit do cursor falhar ou a lease expirar, a mensagem poderá ser publicada novamente. Esse comportamento é intencional e exige idempotência do lado consumidor.

### Componentes e responsabilidades

| Tipo | Responsabilidade |
| --- | --- |
| `OutboxService` | Serializa eventos configurados, aplica/abre proteção e grava ou despacha localmente. |
| `OutboxTracker` | Conecta `IHasEvents.DomainEvents` ao `SavingChanges` do EF Core. |
| `RegisterConsumerHandler` | Cria um cursor nomeado de consumo. |
| `GetMessagesHandler` | Lê mensagens posteriores ao cursor, em ordem, com paginação de 1 a 1.000 itens. |
| `CommitConsumedHandler` | Atualiza diretamente o cursor; adequado para integração controlada/legada, não para o dispatcher concorrente. |
| `IOutboxDispatcherLeaseCoordinator` | Obtém, renova e libera a liderança persistida do dispatcher. |
| `IOutboxDispatcherCursorCoordinator` | Confirma o cursor somente se a lease e o fencing token ainda forem válidos. |
| `IOutboxTransportPublisher` | Fronteira de transporte: recebe uma mensagem persistida sem abrir seu payload. |
| `RabbitMQOutboxTransportPublisher` | Resolve rota tipada e publica o envelope no `IMessageBus`. |
| `OutboxRabbitMQDispatcherService` | Implementação hospedada no `UserAccounts.Api` que coordena polling, lease, publicação, retry e métricas. |

### Modelo persistido

As configurações ficam em `EntityFramework/Mappings/OutboxConfigurations.cs`. Em contextos que aplicam `ApplyLowerCaseConvention()`, como `KeyManagerDbContext`, os nomes físicos usados pelas consultas de coordenação são minúsculos.

| Tabela lógica | Campos relevantes | Regras |
| --- | --- | --- |
| `OutboxMessage` | `Id`, `CreatedAt`, `MessageType`, `VersionType`, `PayloadProtection`, `ProtectionKeyId`, `Payload` | `MessageType` tem até 100 caracteres; `Payload` é `text` obrigatório; `PayloadProtection` nulo identifica um registro legado. |
| `OutboxConsumer` | `Id`, `Name`, `LastConsumedMessageId` | `Name` é único e tem até 100 caracteres; o cursor é um `long`, sem chave estrangeira para a mensagem. |
| `OutboxDispatcherLease` | `DispatcherName`, `OwnerInstanceId`, `LeaseUntil`, `FencingToken` | `DispatcherName` é a chave primária; o token é incrementado quando uma instância diferente assume uma lease expirada. |

As mensagens não são excluídas automaticamente. Isso preserva a possibilidade de novos consumidores iniciarem no começo, mas demanda uma política operacional de retenção antes de a tabela crescer indefinidamente.

## Funcionalidades e regras

### Catálogo de tipos e serialização

Antes de gravar, todo tipo CLR precisa estar configurado em `OutboxOptions`:

```csharp
services.ConfigureOutbox(options =>
{
    options.DefaultPayloadProtection = PayloadProtectionMode.Protected;
    options.AddMessageType<PedidoAprovado>("order-approved");
});
```

`OutboxService.Write` procura o tipo CLR no catálogo. Para tipos não configurados, ele lança `InvalidOperationException`. O metadata guarda:

- nome canônico persistido (`MessageType`);
- versão — o registro de alto nível em `OutboxOptions` cria atualmente a versão `1`;
- `JsonSerializerOptions` específico ou a configuração padrão;
- política de proteção do tipo, ou herança da política padrão da caixa.

A serialização padrão usa JSON compacto com propriedades em `camelCase`.

### Proteção de payload

`PayloadProtectionMode` aceita `Default`, `Protected` e `Plain`:

- `Default` herda `OutboxOptions.DefaultPayloadProtection`;
- `Protected` grava o ciphertext e o identificador da chave vigente;
- `Plain` grava JSON aberto.

O padrão atual de `DefaultPayloadProtection` é **`Plain`**. Eventos que possam conter dados pessoais, credenciais, segredos, identificadores sensíveis ou outros dados protegidos devem configurar `Protected` explicitamente — de preferência como política padrão da caixa.

Para payload protegido, o host deve configurar primeiro o `ProtectionService` corporativo e depois registrar o codec:

```csharp
// A origem das chaves deve ser a configuração corporativa segura do ambiente.
services.AddValueProtection(/* configuração de chaves */);
services.AddProtectedMessageCodec();
```

`AddProtectedMessageCodec()` falha cedo se `ProtectionService` ainda não estiver registrado. Ao ler uma mensagem protegida, o codec usa o `ProtectionKeyId` persistido, permitindo abrir mensagens antigas enquanto a chave anterior continuar disponível.

Mensagens antigas sem `PayloadProtection` somente podem ser lidas localmente por `OutboxService.DispatchAsync` se `LegacyPlainPayloadReadUntilUtc` estiver definido e `CreatedAt` for anterior a esse limite. Já `OutboxTransportEnvelope<TEvent>.From` não aceita mensagens sem essa metadata, portanto registros legados não atravessam o adaptador RabbitMQ até serem tratados por uma estratégia de migração compatível.

### Consumidores e cursor

Um consumidor é um cursor nomeado, não uma fila RabbitMQ. Ele determina a próxima `OutboxMessage` a ser lida:

- `RegisterConsumerRequest.ConsumerName` deve ter de 3 a 100 caracteres e ser único;
- por padrão, um novo consumidor inicia no cursor `0` e pode ler todo o histórico disponível;
- `ConsumeFromLastMessage = true` tenta posicioná-lo na maior mensagem existente, ignorando as anteriores;
- `GetMessages` retorna mensagens com `Id > LastConsumedMessageId`, em ordem crescente;
- o limite aceito pelo handler é de 1 a 1.000; a API HTTP usa `10` quando `limit` é ausente ou não positivo;
- `HasMore` é calculado lendo um item adicional.

`CommitConsumedHandler` apenas atualiza `LastConsumedMessageId` do consumidor existente. Ele não confere se o Id existe, se é posterior ao cursor atual ou se a publicação foi realizada. Para o dispatcher distribuído, use `IOutboxDispatcherCursorCoordinator.TryCommitAsync`, que impõe progresso monotônico, existência da mensagem e lease válida.

### Lease e fencing token

A coordenação está implementada com SQL PostgreSQL no próprio banco da Outbox:

- `TryAcquireAsync` insere ou renova a lease quando ela expirou ou já pertence à mesma instância;
- outro proprietário não consegue adquirir uma lease ainda válida;
- cada troca efetiva de proprietário eleva o `FencingToken`;
- `TryRenewAsync` exige o mesmo nome, proprietário, token e lease ainda não expirada;
- `TryCommitAsync` atualiza o cursor somente se uma lease com os mesmos dados continuar válida no instante do `UPDATE`.

A lease é indexada pelo `DispatcherName`, não pelo `ConsumerName`. Mantenha a associação **um `DispatcherName` estável para um `ConsumerName` compartilhado**. Dois dispatchers com nomes distintos apontando para o mesmo cursor possuem leases independentes e podem concorrer sobre as mesmas mensagens.

### Transporte RabbitMQ

O adaptador registra um catálogo próprio, indexado por `(MessageType, VersionType)`. A rota é tipada porque o canal corporativo é determinado por `OutboxTransportEnvelope<TEvent>`:

```csharp
services.AddOutboxRabbitMQ(
    transport => transport.AddMessageType<PedidoAprovado>("order-approved", versionType: 1),
    rabbit =>
    {
        rabbit.ConfigureConnection(options =>
            options.UseConnectionStringFromConfiguration("RabbitMQ"));

        rabbit.AddChannel<OutboxTransportEnvelope<PedidoAprovado>>(
                ChannelInfo.Topic(
                    exchange: "RoyalCode.pedidos",
                    topic: "order-approved.v1"))
            .UseForPublishOnly();
    });

services.AddRabbitMQHealthCheck();
```

A configuração deve declarar **ambos** os lados do produtor:

1. `transport.AddMessageType<TEvent>(nome, versão)` para o publisher resolver o metadata persistido;
2. `AddChannel<OutboxTransportEnvelope<TEvent>>(...).UseForPublishOnly()` para o `IMessageBus` ter o canal tipado de publicação.

`RabbitMQOutboxTransportPublisher` não desserializa nem abre o payload. Ele publica o envelope abaixo, preservando o JSON aberto ou ciphertext persistido:

```text
OutboxTransportEnvelope<TEvent>
  SourceMessageId      Id sequencial da Outbox
  CreatedAt            instante de criação
  MessageType          nome canônico
  VersionType          versão
  PayloadProtection    Plain ou Protected
  ProtectionKeyId      obrigatório para Protected
  Payload              JSON aberto ou ciphertext original
```

A publicação falha quando não há rota para o par tipo/versão. A conclusão sem exceção de `IOutboxTransportPublisher.PublishAsync` é o sinal que o dispatcher atual trata como confirmação de publicação.

### Relação com Inbox

O consumidor RabbitMQ recomendado recebe `OutboxTransportEnvelope<TEvent>`, valida o tipo, a versão e a política, abre o payload quando necessário e só então o entrega ao `IMessageDispatcher` do Inbox.

Falhas técnicas do envelope — metadata inválida, tipo inesperado, JSON inválido, chave inexistente ou ciphertext corrompido — são rejeitadas pelo `RabbitMQInboxTransportConsumer<TEvent>`. Falhas de negócio nos observers não são escondidas pelo transporte e devem seguir a política do Inbox: redelivery até o limite configurado e persistência como mensagem venenosa quando aplicável.

Para um assinante RabbitMQ que deve receber todos os eventos, use uma fila nomeada própria e habilite dead letter deliberadamente:

```csharp
rabbit.AddChannel<OutboxTransportEnvelope<PedidoAprovado>>(
        ChannelInfo.Topic(
            exchange: "RoyalCode.pedidos",
            topic: "order-approved.v1",
            queueName: "faturamento.order-approved",
            activateDeadLetter: true))
    .UseForReceiveOnly()
    .UseInboxTransportConsumer<PedidoAprovado>()
    .UseExclusiveStrategyForReceive(options => options.PrefetchCount = 5);
```

Não use a mesma fila para consumidores independentes que precisam receber cada evento: eles competirão pela mensagem.

## Como integrar um módulo ao núcleo

### 1. Mapear as tabelas no mesmo DbContext do agregado

```csharp
public sealed class PedidosDbContext : DbContext
{
    public PedidosDbContext(DbContextOptions<PedidosDbContext> options)
        : base(options)
    {
        OutboxTracker.Initialize(this);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyPedidosConfigurations();
        modelBuilder.ApplyOutboxConfigurations();
        modelBuilder.IgnoreHasEvents();
        modelBuilder.ApplyLowerCaseConvention();
    }
}
```

`IgnoreHasEvents()` evita que `DomainEvents` seja mapeado pelo EF Core. O `OutboxTracker` deve ser inicializado no contexto efetivamente usado pelos casos de uso; criar um `OutboxDbContext` separado quebra a atomicidade desejada.

### 2. Registrar serviços e catálogo

```csharp
services.AddDbContext<PedidosDbContext>((sp, options) =>
{
    options.UseNpgsql(sp.GetConnectionString("Pedidos"));
});

services.AddOutboxServices<PedidosDbContext>();

services.AddValueProtection(/* chaves do ambiente */);
services.AddProtectedMessageCodec();

services.ConfigureOutbox(options =>
{
    options.DefaultPayloadProtection = PayloadProtectionMode.Protected;
    options.AddMessageType<PedidoAprovado>("order-approved");
});
```

`AddOutboxServices<TDbContext>()` registra `OutboxService`, o dispatcher local de eventos, handlers de consumidor e os coordenadores de lease/cursor. Ela não chama `OutboxTracker.Initialize` nem configura tipos de mensagem, pois ambas as decisões pertencem ao módulo consumidor.

### 3. Persistir uma mensagem

Em um fluxo controlado, a escrita explícita segue este formato:

```csharp
public sealed class AprovarPedidoHandler
{
    private readonly PedidosDbContext db;
    private readonly OutboxService outbox;

    public AprovarPedidoHandler(PedidosDbContext db, OutboxService outbox)
    {
        this.db = db;
        this.outbox = outbox;
    }

    public async Task AprovarAsync(Pedido pedido, CancellationToken ct)
    {
        pedido.Aprovar();
        outbox.Write(new PedidoAprovado(pedido.Id));

        // Confirma a mudança de domínio e a OutboxMessage no mesmo SaveChanges.
        await db.SaveChangesAsync(ct);
    }
}
```

Para entidades que emitem `IHasEvents.DomainEvents`, o uso preferencial é adicionar o evento no próprio comportamento de domínio e deixar o `OutboxTracker` gravá-lo durante o `SaveChanges`.

### 4. Implementar o dispatcher hospedado

O núcleo oferece os contratos de coordenação, mas não um worker genérico. O dispatcher deve seguir esta sequência, usada por `OutboxRabbitMQDispatcherService` no `UserAccounts.Api`:

1. adquirir a lease para um `DispatcherName` estável;
2. registrar o `ConsumerName` se ele ainda não existir;
3. ler a próxima mensagem do cursor com `GetMessages(Limit = 1)`;
4. chamar `IOutboxTransportPublisher.PublishAsync`;
5. chamar `IOutboxDispatcherCursorCoordinator.TryCommitAsync` com a mesma lease;
6. renovar a lease em paralelo; encerrar a liderança se a renovação falhar;
7. aplicar polling quando não houver mensagens e backoff após falha de publicação.

Não substitua o passo 5 por `ICommitConsumedHandler` em um dispatcher com mais de uma instância.

## Configuração de referência: UserAccounts

O `UserAccounts.Api` valida as opções no startup e registra um `BackgroundService` para publicar seu catálogo. A conexão RabbitMQ é obtida por `ConnectionStrings:RabbitMQ`, que deve ser resolvida pela configuração segura do ambiente; não coloque credenciais no repositório.

```json
{
  "OutboxDispatcher": {
    "DispatcherName": "useraccounts.rabbitmq-publisher",
    "ConsumerName": "useraccounts.rabbitmq-publisher",
    "LeaseDuration": "00:00:30",
    "LeaseRenewInterval": "00:00:10",
    "PollInterval": "00:00:02",
    "MetricsInterval": "00:00:30",
    "FailureBackoff": "00:00:05"
  },
  "OutboxRabbitMQ": {
    "Routes": {
      "scope-approved": {
        "Exchange": "RoyalCode.identidade.escopo",
        "RoutingKey": "scope-approved.v1"
      },
      "credential-created": {
        "Exchange": "RoyalCode.identidade.credencial",
        "RoutingKey": "credential-created.v1"
      }
    }
  }
}
```

As regras de validação de `OutboxDispatcherOptions` exigem nomes não vazios de até 100 caracteres, todos os intervalos positivos e `LeaseRenewInterval < LeaseDuration`. `OutboxRabbitMQOptions` exige uma exchange e uma routing key para **cada** tipo do `UserAccountsEventCatalog`.

O catálogo atual reúne eventos de conta, perfil, escopo e credencial. Ele é a fonte única dos nomes canônicos usados pela Outbox e pelo transporte; adicionar um novo evento requer atualizar o catálogo, a rota RabbitMQ e o canal tipado correspondente.

## API HTTP de leitura

`OutboxApi.MapOutbox()` expõe operações de consumidores no grupo relativo `outbox`. Em um grupo base `/api/v1`, os endpoints são:

| Método e rota | Corpo/parâmetros | Ação |
| --- | --- | --- |
| `POST /api/v1/outbox/consumer` | `{"consumerName":"replica-kms","consumeFromLastMessage":false}` | Registra cursor. |
| `GET /api/v1/outbox/consumer/{consumer}/messages?limit=100` | `limit` opcional | Retorna mensagens posteriores ao cursor. |
| `POST /api/v1/outbox/consumer/{consumer}/commit` | corpo JSON escalar: `42` | Atualiza o último Id consumido. |

A API não aplica autorização, autenticação, rate limit ou política de acesso própria. Esses controles devem estar no `RouteGroupBuilder` pai ou no pipeline do host. Ela serve principalmente a integrações controladas, como o fluxo de federação legado do KeyManager; o dispatcher RabbitMQ atual não depende dela.

## Casos de uso observados

### Publicação de eventos do UserAccounts

O `UserAccounts.Api` registra rotas para seu catálogo e publica, por exemplo, `scope-approved` no tópico `RoyalCode.identidade.escopo` com routing key `scope-approved.v1`. O envelope mantém o `SourceMessageId` da Outbox, o tipo, a versão e o payload persistido para que o receptor possa validar e processar idempotentemente.

### Replicação do KeyManager

O `KeyManagerDbContext` inicializa `OutboxTracker` quando o vault é Master, aplica o mapeamento da Outbox no banco de Configuration e registra os eventos de chaves, tokens e federação em `ConfigureOutboxInbox`. A réplica usa um consumidor registrado e o fluxo HTTP de `OutboxApi` para obter, despachar e confirmar mensagens.

### Retomada após indisponibilidade do broker

No dispatcher UserAccounts, uma exceção em `IOutboxTransportPublisher.PublishAsync` mantém o cursor inalterado, aguarda `FailureBackoff` e tenta novamente. Os testes cobrem uma falha transitória seguida de nova entrega e posterior avanço do cursor.

## Formas incorretas de trabalhar

| Prática incorreta | Consequência | Forma correta |
| --- | --- | --- |
| Publicar diretamente no RabbitMQ dentro do handler de negócio. | Banco e broker podem divergir quando um deles falha. | Persistir uma `OutboxMessage` no mesmo `SaveChanges` e publicar em segundo plano. |
| Gravar a mensagem em outro `DbContext` ou esquecer o `SaveChanges`. | A mensagem não participa da transação de negócio ou simplesmente não é persistida. | Usar o mesmo contexto do agregado e uma única confirmação transacional. |
| Chamar `OutboxService.Write` para tipo não registrado. | A operação lança exceção por metadata ausente. | Registrar todos os eventos em `ConfigureOutbox`. |
| Usar payload `Plain` para informações sensíveis. | Dados ficam em JSON aberto no banco e no transporte. | Configurar `Protected`, `ValueProtection` e `AddProtectedMessageCodec`. |
| Abrir o payload no publisher RabbitMQ. | Amplia exposição de dados e viola a borda de transporte atual. | Publicar o envelope com o payload persistido; abrir somente no Inbox autorizado. |
| Confirmar cursor antes da publicação. | Uma falha posterior perde a mensagem para aquele consumidor. | Publicar primeiro; confirmar cursor somente depois. |
| Usar `CommitConsumedHandler` em cenário distribuído. | O cursor pode retroceder, pular Ids ou ser movido por uma instância sem liderança. | Usar `IOutboxDispatcherCursorCoordinator` com lease e fencing token. |
| Configurar dois `DispatcherName` para o mesmo `ConsumerName`. | As leases não se excluem entre si e pode haver publicação concorrente/duplicada. | Manter uma associação estável de dispatcher para cursor. |
| Declarar a rota no catálogo de transporte, mas não registrar o canal RabbitMQ tipado. | O publisher pode resolver a rota lógica, mas o barramento não terá o canal de publicação correspondente. | Configurar catálogo e `AddChannel<OutboxTransportEnvelope<T>>(...).UseForPublishOnly()`. |
| Reutilizar uma única fila para assinantes independentes. | Os assinantes dividem mensagens, em vez de cada um recebê-las. | Criar uma fila nomeada por assinatura/consumidor. |
| Expor `MapOutbox` sem política de acesso. | Um cliente indevido pode ler histórico ou manipular cursores. | Exigir autenticação/autorização no grupo pai. |

## Limitações e riscos atuais

1. **Entrega ao menos uma vez.** Entre a publicação e o commit do cursor existe uma janela de redelivery. O `SourceMessageId` deve ser usado pelo consumidor como chave de idempotência quando necessário.
2. **Sem descarte automático.** Não há serviço de retenção para `OutboxMessage`, `OutboxConsumer` ou leases antigas. A retenção precisa ser planejada sem remover mensagens ainda necessárias a cursores ativos.
3. **Bloqueio por cabeça de fila.** O dispatcher publica em ordem, uma a uma. Uma mensagem que falha continuamente impede a publicação das seguintes para o mesmo cursor.
4. **Retry simples do produtor.** O dispatcher UserAccounts usa backoff fixo, sem contador, escalonamento exponencial, quarentena ou diagnóstico persistido por mensagem.
5. **Confirmação do broker é uma abstração.** O dispatcher considera `PublishAsync` concluído sem exceção como confirmação. A interface não expressa um resultado de publisher confirm separado; a garantia efetiva depende do `IMessageBus` configurado.
6. **Dependência de PostgreSQL para a coordenação.** As queries de lease usam `ON CONFLICT`, `make_interval`, `CURRENT_TIMESTAMP` e nomes físicos PostgreSQL. Não são portáveis para SQLite ou InMemory.
7. **Versão limitada no caminho de escrita de alto nível.** `OutboxOptions.AddMessageType` registra versão `1`; embora `OutboxTransportOptions` aceite versão explícita e `CreateMessageRequest` aceite qualquer inteiro positivo, não há uma API equivalente de versionamento no catálogo de escrita.
8. **Legado sem metadata de proteção.** Pode ser lido localmente até uma data de corte, mas não é encapsulado pelo transporte RabbitMQ atual.
9. **Registro retroativo em caixa vazia.** `RegisterConsumerHandler` usa `MaxAsync` para `ConsumeFromLastMessage = true` sem um caso explícito para Outbox vazia; trate esse cenário antes de habilitar o modo em produção.
10. **API HTTP permissiva para commit.** O endpoint de commit recebe um `long` escalar e o handler não verifica ordem, existência do Id ou lease.
11. **Cobertura relacional desigual.** Parte dos testes de handlers usa `UseInMemoryDatabase`; o comportamento de lease/cursor deve continuar validado contra PostgreSQL real, como os testes de integração do UserAccounts.

## Observabilidade e operação

O dispatcher do `UserAccounts.Api` registra logs estruturados para aquisição/perda de lease, tentativa, sucesso, falha e confirmação de cursor. Ele também expõe o meter OpenTelemetry:

```text
RoyalCode.UserAccounts.OutboxDispatcher
```

Métricas emitidas:

| Métrica | Tipo | Tags |
| --- | --- | --- |
| `outbox_dispatcher_lease_acquisitions_total` | Counter | `dispatcher` |
| `outbox_dispatcher_lease_losses_total` | Counter | `dispatcher` |
| `outbox_dispatcher_publish_attempts_total` | Counter | `dispatcher`, `message_type`, `version` |
| `outbox_dispatcher_published_messages_total` | Counter | `dispatcher`, `message_type`, `version` |
| `outbox_dispatcher_publish_failures_total` | Counter | `dispatcher`, `message_type`, `version` |
| `outbox_dispatcher_publish_latency_ms` | Histogram | `dispatcher`, `message_type`, `version` |
| `outbox_dispatcher_pending_messages` | Observable gauge | `dispatcher` |
| `outbox_dispatcher_oldest_message_age_seconds` | Observable gauge | `dispatcher` |

Alertas recomendados: crescimento contínuo de mensagens pendentes, idade elevada da mensagem mais antiga, falhas consecutivas de publicação, perda frequente de lease e ausência de publicações quando há backlog.

O RabbitMQ possui health check próprio. Registre `services.AddRabbitMQHealthCheck()` no deployável; o teste `RabbitMQHealthCheckIntegrationTests` confirma que um broker indisponível resulta em estado `Unhealthy`.

## Testes existentes e como interpretá-los

| Área | Evidência principal |
| --- | --- |
| Escrita e proteção | `CreateMessageTests` valida payload aberto, protegido e leitura legada anterior ao cutoff. |
| Consumidor e cursor | `RegisterConsumerTests`, `GetMessagesTests` e `CommitConsumedTests`. |
| Publisher RabbitMQ | `RabbitMQOutboxTransportPublisherTests` confirma que ciphertext/JSON original e metadata são preservados no envelope tipado. |
| Dispatcher distribuído | `OutboxRabbitMQDispatcherServiceIntegrationTests` cobre sucesso, falha, redelivery, lease perdida, renovação e duas instâncias. |
| Transporte e Inbox | `RabbitMQInboxTransportConsumerTests` e os testes de integração de redelivery, falha transitória e poison message. |
| Broker e health check | Testes marcados com `Category = RabbitMQIntegration`; alguns exigem `USERACCOUNTS_TEST_RABBITMQ_PASSWORD`. |

Testes de integração com RabbitMQ não devem ser tratados como testes unitários puros: exigem um broker configurado. Para o módulo UserAccounts, a infraestrutura de integração usa PostgreSQL real configurável no `appsettings.json` do projeto de testes.

## Oportunidades de melhoria

1. Criar um dispatcher reutilizável no projeto Outbox, parametrizado por opções, para reduzir duplicação entre deployáveis.
2. Persistir tentativas, último erro, horário da próxima tentativa e estado de publicação, permitindo backoff exponencial e intervenção operacional.
3. Implementar retenção segura: somente remover mensagens anteriores ao menor cursor relevante e após uma janela de auditoria definida.
4. Adicionar índice/estratégia de consulta para relatórios de backlog por `CreatedAt` quando o volume justificar.
5. Criar API de versionamento no `OutboxOptions` e uma política explícita de compatibilidade de contratos.
6. Endurecer `CommitConsumedHandler` ou restringi-lo a um adaptador legado, verificando progresso monotônico e Id existente.
7. Tratar explicitamente `ConsumeFromLastMessage` quando não existir nenhuma mensagem.
8. Expor health check e métricas genéricos no núcleo, em vez de manter instrumentação somente no UserAccounts.
9. Adicionar testes PostgreSQL específicos para `OutboxDispatcherLeaseCoordinator` e `OutboxDispatcherCursorCoordinator`, pois usam SQL dependente do provider.
10. Formalizar a garantia de confirmação do publisher RabbitMQ e, se suportado pela biblioteca, expor confirmação do broker como resultado explícito.

## Arquivos de referência

- `src/RoyalCode.Outbox/Services/OutboxService.cs`
- `src/RoyalCode.Outbox/EntityFramework/OutboxTracker.cs`
- `src/RoyalCode.Outbox/EntityFramework/OutboxDispatcherLeaseCoordinator.cs`
- `src/RoyalCode.Outbox/EntityFramework/OutboxDispatcherCursorCoordinator.cs`
- `src/RoyalCode.Outbox/EntityFramework/Mappings/OutboxConfigurations.cs`
- `src/RoyalCode.Outbox/Api/OutboxApi.cs`
- `src/RoyalCode.Outbox.RabbitMQ/Extensions/OutboxRabbitMQServiceCollectionExtensions.cs`
- `src/RoyalCode.Outbox.RabbitMQ/Services/RabbitMQOutboxTransportPublisher.cs`
- `src/RoyalCode.UserAccounts.Api/Services/OutboxRabbitMQDispatcherService.cs`
- `src/RoyalCode.UserAccounts.Api/Services/OutboxDispatcherTelemetry.cs`
- `src/RoyalCode.UserAccounts.Api/ServicesExtensions.cs`
- `src/RoyalCode.Outbox.Tests/` e `src/RoyalCode.UserAccounts.Tests/Integration/`

Também foi consultada a referência interna `.ai/references/messaging/RoyalCode-messaging-rabbitmq-usage.md`.
