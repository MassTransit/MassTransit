# Configuring a container

MassTransit does not require a dependency injection container, however, it includes support for the most popular containers. Each container has its own style, and as such usage scenarios may vary.

MassTransit now has a consistent API for registration and configuration when using a container, as well as conventions for configuring receive endpoints based upon the registered consumers, routing slip activities, and sagas. Under the hood, each container is configured to properly interact with MassTransit, leveraging available container features, such as nested scopes for message consumers, without requiring the developer to explictly configure every consumer.

<div class="alert alert-info">
<b>Note:</b>
    Dependency Injection styles are a personal choice that each developer or organization must make on their own. We recognize this choice, and respect it, and will not judge those who don't use a particular container or style of dependency injection. In short, we care.
</div>

## Registration

For containers which support registration, the `.AddMassTransit` extension method is used. There are methods supporting all consumer types, including consumers, saga, Courier activities, and saga state machines (using the container-specific extension library for Automatonymous available on [NuGet](https://www.nuget.org/packages?q=id:MassTransit.Automatonymous.*&prerelease=false)).

## Definition

In addition to registration, MassTransit supports an optional definition for the consumer, which can be specified (or discovered) via a class in the assembly. The syntax is under development, but some basic features are working today.

```csharp
        public class SubmitOrderConsumerDefinition :
            ConsumerDefinition<SubmitOrderConsumer>
        {
            public SubmitOrderConsumerDefinition()
            {
                // override the default endpoint name, for whatever reason
                EndpointName = "ha-submit-order";

                // specify a concurrency limit for this consumer
                ConcurrencyLimit = 4;

                // this is under development, doesn't do anything yet!
                // but will eventually be used to define service endpoints
                // in conductor.
                Request<SubmitOrder>(x =>
                {
                    x.PartitionBy(m => m.CustomerId);

                    x.Publishes<OrderReceived>();
                    x.Responds<OrderAccepted>();
                    x.Responds<OrderRejected>();
                    x.Sends<ProcessOrder>();
                });
            }

            protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
                IConsumerConfigurator<DiscoveryPingConsumer> consumerConfigurator)
            {
                endpointConfigurator.UseMessageRetry(r => r.Interval(5,1000));
                endpointConfigurator.UseInMemoryOutbox();
            }
        }
```

There are definitions for all consumer types, including sagas, activities, and saga state machines. They can be registered explicity using the `.AddConsumer(consumer type, consumerDefinitionType)` methods, or can be discovered using the scanning methods, such as `.AddConsumersFromNamespaceContaining<SubmitOrderConsumer>()`. 


**Hey! Where is my container??**

Containers come and go, so if you don't see your container here, or feel that the support for you container is weaksauce, pull requests are always welcome. Using an existing container it should be straight forward to add support for your favorite ÜberContainer.

* [Autofac](autofac.md)
* [Ninject](ninject.md)
* [StructureMap](structuremap.md)
* [Lamar](lamar.md)
* [Unity](unity.md)
* [Castle Windsor](castlewindsor.md)
* [Microsoft Dependency Injection](msdi.md)
