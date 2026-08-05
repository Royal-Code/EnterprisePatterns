# Versionamento de contratos de mensagem

## Estado atual

`OutboxOptions.Types` e indexado pelo tipo CLR. `AddMessageType<T>` registra somente uma entrada por tipo e fixa `Version = 1`. Na leitura, o sistema procura o par `(TypeName, Version)` percorrendo a colecao. Portanto, a coluna `VersionType` existe, mas ainda nao ha uma API nem um registro apropriado para coexistencia de versoes.

## Objetivo

Permitir que produtores passem de v1 para v2 sem alterar mensagens persistidas e que consumidores leiam v1 e v2 durante a janela de retencao. O contrato persistido e o par `(contractName, version)`. Ele deve ser imutavel. O tipo CLR e apenas a representacao usada por uma aplicacao para produzir ou ler uma versao desse contrato.

## Opcao A: ampliar o dicionario por tipo

Adicionar `version` a `AddMessageType<T>` resolve a gravacao de uma versao por tipo, mas nao permite associar o mesmo tipo CLR a duas versoes nem otimiza a leitura. E a menor mudanca, mas deixa ambigua a escolha da versao de saida.

Nao recomendada como design final.

## Opcao B: registros separados de escrita e leitura

Manter dois indices internos, ambos validados no startup:

| Indice | Chave | Uso |
| --- | --- | --- |
| writer | `Type` | define o contrato/version produzido por um tipo CLR |
| reader | `(contractName, version)` | desserializa mensagens historicas |

API sugerida, mantendo a sobrecarga atual como atalho para v1:

```csharp
options.AddMessageType<OrderApprovedV1>("order-approved", version: 1);
options.AddMessageType<OrderApprovedV2>("order-approved", version: 2);
options.Publish<OrderApprovedV2>("order-approved", version: 2);
```

`AddMessageType<T>` registra a capacidade de leitura. `Publish<T>` define a unica rota padrao de escrita daquele tipo CLR. Se o mesmo modelo deve ser publicado em mais de uma versao, a chamada de escrita deve informar explicitamente o contrato/version, em vez de escolher de forma implicita.

A implementacao deve expor `IReadOnlyDictionary` ou metodos de consulta, nao o dicionario mutavel atual. A busca de leitura passa a ser O(1), sem `FirstOrDefault`.

Essa e a opcao recomendada: explicita, fluente e pequena.

## Opcao C: builder de contrato

Uma API mais declarativa pode agrupar versoes do mesmo contrato:

```csharp
options.AddContract("order-approved", contract => contract
    .Version<OrderApprovedV1>(1)
    .Version<OrderApprovedV2>(2)
    .Publish<OrderApprovedV2>(2));
```

Ela melhora a leitura de configuracoes grandes, mas introduz um builder novo. Pode ser adicionada futuramente sobre a opcao B; nao deve ser pre-requisito.

## Migracao v1 para v2

1. Criar um DTO/contrato v2 quando a mudanca for incompativel.
2. Registrar v1 e v2 para leitura em produtores e consumidores que atravessem a transicao.
3. Alterar apenas a rota de escrita para v2; mensagens v1 existentes nao sao alteradas.
4. Manter leitores de v1 ate expirar a retencao ou ate uma migracao operacional.
5. Se for necessario reenviar historico como v2, ler v1, transformar explicitamente e criar uma nova mensagem v2 com novo `OutboxMessage.Id` e referencia de causacao para a original.

Nao fazer migracao JSON silenciosa dentro do serializador. Uma transformacao de contrato e uma decisao de negocio/operacao, precisa ser testavel e auditavel.

## Regras de validacao

- `contractName` obrigatorio e estavel.
- `version` maior que zero.
- Nao permitir duplicata de `(contractName, version)`.
- Nao permitir mais de uma rota padrao de escrita para o mesmo tipo CLR.
- Exigir `JsonTypeInfo` compativel com `T` quando ele for informado.
- Falhar no bootstrap, antes de produzir ou consumir mensagens.