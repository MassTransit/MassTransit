How are subscriptions shared?
=============================

Subscriptions are created and then shared to every other bus instance. Though the data 
is the same, how they get to all of the nodes is different depending on your configuration.

MSMQ Multicast
--------------

.. warning::

    - limited by default to one subnet
    - subscriptions do not survive service restarts
    - sensitive to the order in which services are brought online

Each bus instance communicates with every other instance on the network through a reliable
multicast network protocol.

MSMQ Runtime Services
---------------------

Each bus instance communicates with every other intance through an intermediary known as
the Runtime Services (specifically the Subscription Service). 

RabbitMQ
--------

Each bus instance communicates with a local rabbitmq server. Setting up the necessary
bindings and queues based on MassTransit conventions. RabbitMQ then syncs all binding
information to all nodes in the cluster.