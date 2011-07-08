Some MassTransit Terminology
===================================================

Receiver is anything that receives a message. MassTransit
currently has 4 receiver types.

Handlers
--------

The simplest one. Maps a function to an incoming message.

Instances
---------

Maps a function on an object instance to an incoming message.

Consumers
---------

Maps a class that implements an interface. Typically stateless, 
can consumer more than one message type.

Sagas
-----

Stateful multi-message consumers
