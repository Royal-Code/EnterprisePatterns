# Entities

An entity must have two characteristics:
+ It must be persistent, its information cannot be lost during its life cycle.
+ It must have a unique identifier, and it must be the same across contexts, 
since it identifies the same entity.

This means that the same entity may be in several contexts, it may have different properties
in each of these contexts, but it is possible to identify it between these contexts. 
So there should be a unique identifier that is the same for the entity across all contexts.

---

Many objects represent a thread of continuity and identity, going through a lifecycle, though
their attributes may change.

Some objects are not defined primarily by their attributes. They represent a thread of identity
that runs through time and often across distinct representations. Sometimes such an object
must be matched with another object even though attributes differ. An object must be
distinguished from other objects even though they might have the same attributes. Mistaken
identity can lead to data corruption.

Therefore:

When an object is distinguished by its identity, rather than its attributes, make this primary
to its definition in the model. Keep the class definition simple and focused on life cycle
continuity and identity.

Define a means of distinguishing each object regardless of its form or history. Be alert to
requirements that call for matching objects by attributes. Define an operation that is
guaranteed to produce a unique result for each object, possibly by attaching a symbol that
is guaranteed unique. This means of identification may come from the outside, or it may be
an arbitrary identifier created by and for the system, but it must correspond to the identity
distinctions in the model.

The model must define what it means to be the same thing.

From Domain-Driven Design Reference, by Eric Evans.

This work is licensed under the Creative Commons Attribution 4.0 International License.
To view a copy of this license, visit http://creativecommons.org/licenses/by/4.0/.

---

**Identity Field**

Saves a database ID field in an object to maintain identity between an in-memory object and a database row.

Relational databases tell one row from another by using key - in particular, the primary key.
However, in-memory objects don't need such a key, as the object system ensures the correct 
identity under the covers (or in C++'s case with raw memory locations). 
Reading data from a database is all very well, but in order to write data back you
need to tie the database to the in-memory object system.

In essence, Identity Field is mind-numbingly simple. 
All you do is store the primary key of the relational database table in the object's fields.

---

**Portuguese**

Uma entidade deve ter duas características:
+ Deve ser persistente, suas informações não podem se perder durante seu ciclo de vida.
+ Deve ter um identificador único, e ele deve ser o mesmo entre vários contextos, 
uma vez que identifica a mesma entidade.

Isso quer dizer que uma mesma entidade pode estar em vários contextos, 
ela pode ter propriedades diferentes em cada um destes contextos, 
mas é possível identificá-la entre estes contextos. 
Então deverá existir um identificador único que seja igual para a entidade entre todos os contextos.

--

Muitos objetos representam um linha no tempo de continuidade e identidade, 
passando por um ciclo de vida, embora seus atributos possam mudar.

Alguns objetos não são definidos em primeiro lugar por seus atributos. 
Eles representam uma linha identitária que atravessa o tempo e muitas vezes através 
de representações distintas. Às vezes, tal objeto deve ser combinado com outro objeto,
mesmo que os atributos sejam diferentes. Um objeto deve ser distinguido de outros objetos,
mesmo que eles possam ter os mesmos atributos. 
Uma identidade equivocada pode levar à corrupção de dados.

Portanto,

Quando um objeto se distingue por sua identidade, ao invés de seus atributos, 
faça com que isso seja primário para sua definição no modelo. Mantenha a definição 
de classe simples e focada na continuidade do ciclo de vida e na identidade.

Defina um meio de distinguir cada objeto, independentemente de sua forma ou história. 
Esteja atento a requisitos que exigem a identificação de objetos por atributos. 
Defina uma operação que garanta a produção de um resultado único para cada objeto, 
possivelmente anexando um símbolo que é garantidamente único. Este meio de identificação 
pode vir de fora, ou pode ser um identificador arbitrário criado pelo e para o sistema,
mas deve corresponder às distinções de identidade no modelo.

O modelo deve definir o que significa ser a mesma coisa.

--

**Identity Field**

Salva um campo de identificação do banco de dados em um objeto para manter a identidade entre um objeto em memória e uma linha do banco de dados.

As bases de dados relacionais distinguem uma linha da outra utilizando a chave - em particular,
a chave primária. Entretanto, os objetos na memória não precisam de tal chave,
pois o sistema de objetos garante a identidade correta sob as proteções 
(ou no caso de C++ com locais de memória bruta). 
A leitura de dados de um banco de dados está tudo muito bem, 
mas para escrever os dados de volta é necessário ligar o banco de dados ao sistema de objetos em memória.

Em essência, o Campo de Identidade é simples e sem limites mentais. 
Tudo que você faz é armazenar a chave primária da tabela do banco de dados relacional nos campos do objeto.
