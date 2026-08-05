# Protecao de payload

## Objetivo

Permitir que um contrato de evento seja gravado como JSON aberto ou como ciphertext, sem expor chaves ao nucleo da Outbox e sem obrigar o adaptador de transporte a abrir o payload.

## Opcao minima

Manter apenas JSON aberto e delegar criptografia ao banco ou ao broker. E simples, mas nao protege copias de banco, backups, dumps e encaminhamentos que leiam a tabela. Nao atende eventos que carregam dados sensiveis.

## Design recomendado

Adicionar no nucleo um contrato pequeno, `IOutboxPayloadCodec`, responsavel por transformar JSON em um valor persistivel e por abrir o valor apenas no dispatch local autorizado.

```text
serializador JSON -> IOutboxPayloadCodec.Encode -> OutboxMessage
OutboxMessage -> publisher de transporte -> envelope opaco
OutboxMessage -> IOutboxPayloadCodec.Decode -> dispatch local
```

O resultado da codificacao contem `Payload`, `PayloadProtection` e `ProtectionKeyId`. Os modos iniciais sao `Plain`, `Protected` e `Default`; `Default` herda a politica global e pode ser sobrescrito por contrato.

O codec protegido pertence a uma integracao separada e usa o servico corporativo de protecao/chaves. Ele deve usar criptografia autenticada fornecida por essa dependencia, registrar o identificador da chave e falhar no startup quando algum contrato protegido estiver configurado sem codec disponivel.

## Regras

- Serializar antes de proteger e preservar o ciphertext como `Payload`.
- O publisher RabbitMQ/Kafka nunca desserializa nem descriptografa o payload.
- O consumidor autorizado valida modo e `ProtectionKeyId` antes de abrir o conteudo.
- Uma chave antiga precisa continuar disponivel enquanto houver mensagens que a referenciem.
- Mensagens antigas sem metadado de protecao seguem uma politica explicita de leitura legada com data de corte; nao devem ser aceitas implicitamente para sempre.

## Performance

Nao ha decrypt/re-encrypt durante a publicacao. O custo no produtor e uma serializacao e, quando necessario, uma operacao de protecao. O codec deve reutilizar o provider de chaves e evitar serializar o mesmo objeto duas vezes. O payload permanece `text`/UTF-8 na primeira versao para preservar o schema e os adaptadores atuais.

## Decisao

Adicionar os metadados de protecao junto da evolucao de `OutboxMessage` descrita em [an-message-metadata.md](an-message-metadata.md). Manter o codec como extensao opcional, mas recomendar `Protected` como politica padrao dos deploys que publicam dados sensiveis.