# MongoDB

MongoDB is easy to setup as a saga repository. MassTransit includes sensible defaults, and there is no need to explicitly map sagas.

Storing a saga in MongoDB requires an additional interface, _ISagaVersion_, which has a _Version_ property used for optimistic concurrency. An example saga is shown below.

<<< @/docs/code/sagas/OrderState.cs

### Configuration

To configure MongoDB as a saga repository, use the code shown below using the _AddMassTransit_ container extension. This will configure MongoDB to connect to the local MongoDB instance on the default port using Optimistic concurrency. The _CorrelationId_ property will be automatically mapped to be the document identifier.

<<< @/docs/code/sagas/MongoDbSagaContainer.cs

In the example above, saga instances are stored in a collection named `order.states`. The collection name can be specified using the _CollectionName_ property. Alternatively, a collection name formatter can be specified using the _CollectionNameFormatter_ method.

<<< @/docs/code/sagas/MongoDbSagaContainerCollection.cs

Container integration gives you ability to configure class map based on saga type. You can use `Action<BsonClassMap>` explicitly:

<<< @/docs/code/sagas/MongoDbSagaClassMap.cs

`BsonClassMap<TSaga>` registered inside container will be used by default for `TSaga` configuration:

<<< @/docs/code/sagas/MongoDbRegisterClassMap.cs
