Middleware
""""""""""""""""""""""""

MassTransit middleware follows the pattern that we are seeing in many web
frameworks such as Owin, Rack, and Connect. All middleware by convention starts
with a ``Use`` and are implemented as extension methods off of ``IPipeConfigurator<T>``.

.. toctree::

    transactions.rst
    logging.rst
    circuit_breaker.rst
    custom.rst

