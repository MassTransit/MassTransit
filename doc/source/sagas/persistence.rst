Persisting Saga Instances
=========================

.. attention:: **This page is obsolete!**

   New documentation is located at http://masstransit-project.com/MassTransit.

   The latest version of this page can be found here_.

.. _here: http://masstransit-project.com/MassTransit/advanced/sagas/persistence.html

Sagas are stateful event-based message consumers -- they retain state. Therefore, saving state between events is important. Without persistent state, a saga would consider each event a new event, and orchestration of subsequent events would be meaningless.

Identity
--------

Saga instances are identified by a unique identifier (``Guid``), represented by the ``CorrelationId`` on the saga instance. Events are correlated to the saga instance using either the unique identifier, or alternatively using an expression that correlates properties on the saga instance to each event. If the ``CorrelationId`` is used, it's always a one-to-one match, either the saga already exists, or it's a new saga instance. With a correlation expression, the expression might match to more than one saga instance, so care should be used -- because the event would be delivered to all matching instances.


.. note::

    Seriously, don't sent an event to all instances -- unless you want to watch your messages consumers lock your entire saga storage engine.

Publishing and Sending From Sagas
---------------------------------

Sagas are completely message-driven and therefore not only consume but also publish events and send commands. However, if your saga received a lot of messages coming roughly at the same time and the endpoint is set to process multiple messages in parallel - this can lead to a conflict between message processing and saga persistence.

This means that there could be more than one saga state updates that are being persisted at the same time. Depending on the saga repository type, this might fail for different reasons - versioning issue, row or table lock or eTag mismatch. All those problems are basically saying that you are having a concurrency issue.

It is normal for the saga repository to throw an exception in such case but if your saga is publishing messages, they were already published but the saga state has not been updated. MassTransit will eventually use retry policy on the endpoint and more messages will be send, potentially leading to mess. Or, if there are no retry policies configured, messages might be sent indicating that the process needs to continue but saga instance will be in the old state and will not accept any further messages because they will come in a wrong state.

This issue is common and can be solved by postponing the message publish and send operations until all persistence work is done. All messages that should be published, are collected in a buffer, which is called "outbox". MassTransit implements this feature and it can be configured by adding these lines to your endpoint configuration:

.. sourcecode:: csharp
    :linenos:

	c.ReceiveEndpoint("queue", e =>
	{
	    e.UseInMemoryOutbox();
	    // other endpoint configuration here
	}

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

Redis
~~~~~

Redis is a very popular key-value store, which is known for being very fast.

Redis does not support queries, therefore Redis saga persistence only supports correlation by id. If you try to use correlation by expressions, you will get a "not implemented" exception.

Saga persistence for Redis uses ``ServiceStack.Redis`` library and it support both BSD-licensed v3.9.71 and the latest commercial versions as well.

Saga instance class must implement ``IHasGuid`` interface and the ``Id`` property, that must return the value of the ``CorrelationId`` property:

.. sourcecode:: csharp
    :linenos:

    public class SagaInstance : SagaStateMachineInstance, IHasGuidId
    {
        public Guid CorrelationId { get; set; }
        public Guid Id => CorrelationId;
        public string CurrentState { get; set; }

        public string CustomData { get; set; }
    }
 
 Redis saga persistence does not aquire locking on the database record when writing it so potentially you can have write conflict in case the saga is updating its state frequently (hundreds of times per second). To resolve this, the saga instance can implement the ``IVersionedSaga`` inteface and include the Version property:

.. sourcecode:: csharp

    public int Version { get; set; }

When version of the instance that is being updated will be lower than the expected version, the saga repository will trow an exception and force the message to be retried, potentially resolving the issue.

The Redis saga repository requires ``ServiceStack.Redis.IRedisClientsManager`` as constructor parameter. For containerless initialization the code would look like:

.. sourcecode:: csharp
    :linenos:

    var redisConnectionString = "redis://localhost:6379";
    var repository = new RedisSagaRepository<SagaInstance>(new RedisManagerPool(redisConnectionString));


If you use a container, you can use the code like this (example for Autofac):

.. sourcecode:: csharp
    :linenos:

    var redisConnectionString = "redis://localhost:6379";
    builder.Register<IRedisClientsManager>(c => new RedisManagerPool(redisConnectionString)).SingleInstance();
    builder.RegisterGeneric(typeof(RedisSagaRepository<>)).As(typeof(ISagaRepository<>)).SingleInstance();

