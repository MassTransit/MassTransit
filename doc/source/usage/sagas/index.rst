Sagas
=================

Sagas, or distributed workflows, are a powerful feature provided by
MassTransit to accompolish coordinated, long-lived and distributed transactions.
Like most powerful features there is quite a bit of ground work
that needs to be covered to really understand and leverage them correctly.

At their core, Sagas are really just fancy state machines, because of this
the developers of MassTransit have leveraged existing state machine library
called Automatonymous_. Automatonymous is a
state machine engine written by Chris Patterson, aka, PhatBoyG.

.. _Automatonymous: https://github.com/MassTransit/Automatonymous

What is a saga?
----------------

A saga is a long-lived transaction managed by a coordinator. Sagas are stateful
entities which orchestrate events, changing state as events are observed, and
maintaining the overall state of the transaction. Sagas were designed to manage
the complexity of a distributed transaction without locking or immediate consistency.
They manage state and are able to identify when a transaction completed successfully
or if it faulted, necessitating compensation.

We didn’t create it, we learned it from the `original Cornell paper`_ and from Arnon Rotem-Gal-Oz's `description`_.

.. _original Cornell paper: http://www.cs.cornell.edu/andru/cs711/2002fa/reading/sagas.pdf
.. _description: http://www.rgoarchitects.com/Files/SOAPatterns/Saga.pdf



.. toctree::

    correlation_ids.rst
    automatonymous.rst
