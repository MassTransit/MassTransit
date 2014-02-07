Inheritance and Message Class Design
====================================

That said, I would advise you to think about the following things:

#. Interface-based inheritance is OK
#. Class-based inheritance is to be approached with caution
#. Composing messages together ends up pushing us into content-based routing which is something we don't recommend
#. Message Design is not OO Design (A message is just state, no behavior) There is a greater focus on interop and contract design.
#. As messages are more about contracts, we suggest subscribing to interfaces that way you can easily evolve the implementation of the message.
#. A big base class may cause pain down the road as each change will have a larger ripple. This can be especially bad when you need to support multiple versions.
