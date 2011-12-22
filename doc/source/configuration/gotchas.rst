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