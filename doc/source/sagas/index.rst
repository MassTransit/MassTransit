Sagas
=====

The ability to ochestrate a series of events is a powerful feature, and MassTransit makes this possible.


What is a saga?
---------------

A saga is a long-lived transaction managed by a coordinator. Sagas are initiated by an event, sagas orchestrate events, and sagas maintain the state of the overall transaction. Sagas are designed to manage the complexity of a distributed transaction without locking and immediate consistency. They manage state and track any compensations required if a partial failure occurs.

We didn’t create it, we learned it from the `original Cornell paper`_ and from Arnon Rotem-Gal-Oz's `description`_.

.. _original Cornell paper: http://www.cs.cornell.edu/andru/cs711/2002fa/reading/sagas.pdf
.. _description: http://www.rgoarchitects.com/Files/SOAPatterns/Saga.pdf


State machine sagas
-------------------

At their core, sagas are just fancy state machines. Because of this, MassTransit uses the standalone state machine library
called Automatonymous_. Automatonymous is a state machine engine written by Chris Patterson, aka, PhatBoyG.

.. _Automatonymous: https://github.com/MassTransit/Automatonymous


.. toctree::

    automatonymous.rst
    quickstart.rst
    samplestatemachine.rst
    persistence.rst
    
