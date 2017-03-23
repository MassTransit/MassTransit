Using Sagas
===========

.. attention:: **This page is obsolete!**

   New documentation is located at http://masstransit-project.com/MassTransit.

   The latest version of this page can be found here_.

.. _here: http://masstransit-project.com/MassTransit/advanced/sagas/

The ability to ochestrate a series of events is a powerful feature, and MassTransit makes this possible.

A saga is a long-lived transaction managed by a coordinator. Sagas are initiated by an event, sagas orchestrate events, and sagas maintain the state of the overall transaction. Sagas are designed to manage the complexity of a distributed transaction without locking and immediate consistency. They manage state and track any compensations required if a partial failure occurs.

We didn’t create it, we learned it from the `original Cornell paper`_ and from Arnon Rotem-Gal-Oz's `description`_.

.. _original Cornell paper: http://www.cs.cornell.edu/andru/cs711/2002fa/reading/sagas.pdf
.. _description: http://www.rgoarchitects.com/Files/SOAPatterns/Saga.pdf


.. toctree::

    automatonymous.rst
    persistence.rst
    
