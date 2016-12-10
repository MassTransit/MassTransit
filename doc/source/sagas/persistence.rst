Persisting Saga Instances
=========================

Sagas are stateful event-based message consumers -- they retain state. Therefore, saving state between events is important. Without persistent state, a saga would consider each event a new event, and orchestration of subsequent events would be meaningless.

Identity
--------

Saga instances are identified by a unique identifier (``Guid``), represented by the ``CorrelationId`` on the saga instance. Events are correlated to the saga instance using either the unique identifier, or alternatively using an expression that correlates properties on the saga instance to each event. If the ``CorrelationId`` is used, it's always a one-to-one match, either the saga already exists, or it's a new saga instance. With a correlation expression, the expression might match to more than one saga instance, so care should be used -- because the event would be delivered to all matching instances.


.. note::

    Seriously, don't sent an event to all instances -- unless you want to watch your messages consumers lock your entire saga storage engine.


Storage Engines
---------------

MassTransit supports several storage engines, including NHibernate, Entity Framework, and MongoDB. Each of these are setup in a similar way, but examples are shown below for each engine.

Entity Framework
~~~~~~~~~~~~~~~~

Entity Framework seems to be the most common ORM for class-SQL mappings, and SQL is still widely used for storing data. So it's a win to have it supported out of the box by MassTransit. The code-first mapping example below shows the basics of getting started.

.. sourcecode:: csharp
    :linenos:

    public class SagaInstance :
        SagaStateMachineInstance
    {
        public SagaInstance(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        protected SagaInstance()
        {
        }

        public string CurrentState { get; set; }
        public string ServiceName { get; set; }
        public Guid CorrelationId { get; set; }
    }


    public class SagaInstanceMap :
        SagaClassMapping<SagaInstance>
    {
        public SagaInstanceMap()
        {
            Property(x => x.CurrentState);
            Property(x => x.ServiceName, x => x.Length(40));
        }
    }

The repository is then created on the context factory for the ``DbContext`` is available.

.. sourcecode:: csharp

    SagaDbContextFactory contextFactory = () => 
        new SagaDbContext<SagaInstance, SagaInstanceMap>(_connectionString);

    var repository = new EntityFrameworkSagaRepository<SagaInstance>(contextFactory);


MongoDB
~~~~~~~

MongoDB is an easy to use saga repository, because setup is easy. There is no need for class mapping, the saga instances can be persisted easily using a MongoDB collection.

.. sourcecode:: csharp
    :linenos:

    public class SagaInstance :
        SagaStateMachineInstance
    {
        public SagaInstance(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        protected SagaInstance()
        {
        }

        public string CurrentState { get; set; }
        public string ServiceName { get; set; }
        public Guid CorrelationId { get; set; }
    }

The saga repository is created using the simple syntax:

.. sourcecode:: csharp

    var database = new MongoClient("mongodb://127.0.0.1").GetDatabase("sagas");
    var repository = new MongoDbSagaRepository<SagaInstance>(database);

Each saga instance will be placed in a collection specific to the instance type.


NHibernate
~~~~~~~~~~

Although NHibernate is not being actively developed recently, it is still widely used and is supported by MassTransit for saga storage. The example below shows the code-first approach to using NHibernate for saga persistence.

.. sourcecode:: csharp
    :linenos:

    public class SagaInstance :
        SagaStateMachineInstance
    {
        public SagaInstance(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        protected SagaInstance()
        {
        }

        public string CurrentState { get; set; }
        public string ServiceName { get; set; }
        public Guid CorrelationId { get; set; }
    }


    public class SagaInstanceMap :
        SagaClassMapping<SagaInstance>
    {
        public SagaInstanceMap()
        {
            Property(x => x.CurrentState);
            Property(x => x.ServiceName, x => x.Length(40));
        }
    }

The ``SagaClassMapping`` base class maps the ``CorrelationId`` of the saga, and handles some of the basic bootstrapping of the class map. All of the properties, including the property for the ``CurrentState`` (if you're using state machine sagas), must be mapped by the developer. Once mapped, the ``ISessionFactory`` can be created using NHibernate directly. From the session factory, the saga repository can be created.

.. sourcecode:: csharp

    ISessionFactory sessionFactory = CreateSessionFactory();
    var repository = new NHibernateSagaRepository<SagaInstance>(sessionFactory);





