Comman Gotcha's
===============

Trying to share a queue
"""""""""""""""""""""""

Each application needs it own address! If you have a website and a console application they will
each need their own address. For instance the website could listen at ``msmq://localhost/web`` and
the console at ``msmq://localhost/console``.


How to do an NServiceBus send only endpoint?
""""""""""""""""""""""""""""""""""""""""""""

Use the IEndpointResolver to get an Endpoint that you can call ``.Send(msg)`` on.


How to setup a competing consumer?
""""""""""""""""""""""""""""""""""

need to doc this. ;)

RabbitMQ and Msmq

Where did the Batch<T> go?
""""""""""""""""""""""""""

We removed the concept, as it required you to bind to the MT dll too 
tightly in your messages. We don't yet have a way to reproduce the 
behavior but you can do this in your application. If you would like
to submit an example we would appreciate it. :)

So, what links two bus instances together?
""""""""""""""""""""""""""""""""""""""""""

This is a common question. The binding element, really is the 
message contract. If you want message A, then you subscribe to 
message A. The internals of MT wires it all together.