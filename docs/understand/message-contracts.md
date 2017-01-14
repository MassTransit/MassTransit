# Inheritance and message class design

That said, I would advise you to think about the following things:
1. Interface-based inheritance is OK.
1. Class-based inheritance is to be approached with caution.
1. Composing messages together ends up pushing us into content-based routing which is something we don't recommend.
1. Message Design is not OO Design (A message is just state, no behavior). 
   There is a greater focus on interop and contract design.
1. As messages are more about contracts, we suggest subscribing to interfaces that way you can easily 
   evolve the implementation of the message.
1. A big base class may cause pain down the road as each change will have a larger ripple. 
   This can be especially bad when you need to support multiple versions.
