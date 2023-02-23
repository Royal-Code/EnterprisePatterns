# Aggregate Domain Pattern

Em desenvolvimento...

## Introdução

O Agregado é um conceito do Domain Driven Design.

Este building block é mais conceitual do que implementável, mas ele influencia no design e na implementação.

O Agregado é um conjunto de entidades e objetos de valor que possuem uma coesão determinada pelo domínio, 
onde uma das entidades é considerada a raiz, e todas as relações entre os objetos de valor e outras entidades possuem uma dependência com a entidade raiz.

Um objeto de valor é uma instância de uma classe (tipo de dado) que possui uma representação no domínio, ou seja, 
é capaz de guardar uma informações de negócio. Estes objetos de valor podem ser anêmicos ou ricos,
os quais trazem consigo as regras de negócio relacionadas ao valor que eles representam.

Os objetos de valor não possuem identificador único como uma entidade, eles são comparados pelo seu valor (propriedades, campos).
Assim, um objeto de valor não tem um estado que pode ser alterado, pois ao alterar uma propriedade ou campo o objeto se torna outro
por seu valor represetar outra coisa, isto muda seu significado.

Já as entidades possuem um identificador para elas, um ID, e tem seu estado persistido e alterado durante o tempo de vida dela.

Um agregado deverá ter uma entidade que seja a raiz, o aggregate root.

O ID da entidade raiz é o ID do agregado.
Os repositórios deverão ser criados por agregado, ou seja, por entidade raiz do agregado.
As entidades internas do agregado não devem ter repositórios para elas, elas devem ser acessadas apenas através da entidade raiz.

A entidade raiz terá relacionamentos com todos componentes do agregado, outras entidade e objetos de valor.
Todas as operações, seja para leitura, como buscar uma entidade interna do agregado, ou de operações que alterem o estado,
devem passar pela entidade raiz.

Deverão existir métodos na entidade raiz para realizar as operações. Assim, as regras de negócio relacionadas as operações
são implementadas internamente no agregado. O resultado é uma melhor organização do código, 
e um código que se possa entender melhor o negócio que ele implementa.

Para cada operação que é realizada no agregado, pode ser gerado um evento de domínio.
Estes eventos são coisas importantes que aconteceram, ou seja, a operação foi executada.
Os eventos de domínio são coisas importam para o negócio, onde pessoas, processos, máquinas, envolvidas no negócio precisam
ser notificadas.

É responsabilidade da entidade raiz gerar ou gerenciar estes eventos de domínio.

## Implementação

Esta biblioteca contém abstrações e interfaces para classes que representam raizes de agregados.

A interface `IAggregateRoot` representa a raiz de um agregado, herdando `IEntity` do projeto <kbd>RoyalCode.DomainEvents</kbd> e `IHasEvents` do projeto <kbd>RoyalCode.DomainEvents</kbd>.

A classe `AggregateRoot` contém a implementação base para uma entidade raiz de agregado.
