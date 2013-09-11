Using MassTransit with an IoC Container
"""""""""""""""""""""""""""""""""""""""

MassTransit has been built from the beginning with the concept of an IoC container
being involved. Our support for using them is quite solid and mature and is most certainly
recommended. However with everything else you are learning trying to figure out 
just how to get the container involved might be overwhelming. Below you will find prototypical
examples of container integration.


StructureMap
''''''''''''

.. sourcecode:: csharp

    public static void main(string[] args) 
    {
        var container = new Container(cfg =>
        {
            // register each consumer
            cfg.ForConcreteType<YourConsumer>();
            
            //or use StructureMap's excellent scanning capabilities
        });
        
        var bus = ServiceBusFactory.New(sbc =>
        {
            //other configuration options
            
            //this will find all of the consumers in the container and 
            //register them with the bus.
            sbc.Subscribe(x => x.LoadFrom(container));
        });
        
        //now we add the bus
        container.Inject(bus);
    }

.. note::

    We recommend that most of this type of code be placed in an StructureMap Registry
    
Windsor
'''''''

.. sourcecode:: csharp

    public static void main(string[] args) 
    {
        var container = new WindsorContainer();
        
        // register each consumer manually
        container.Register(Component.For<YourConsumer>().LifestyleTransient());
        
        //or use Windsor's excellent scanning capabilities
        container.Register(AllTypes.FromThisAssembly().BasedOn<IConsumer>());
        
        var bus = ServiceBusFactory.New(sbc =>
        {
            //other configuration options
            
            //this will find all of the consumers in the container and 
            //register them with the bus.
            sbc.Subscribe(x => x.LoadFrom(container));
        });
        
        //now we add the bus
        container.Register(Component.For<IServiceBus>().Instance(bus));
    }

.. note::

    We recommend that most of this type of code be placed in an IWindsorInstaller

A POCO handler approach and IWindsorInstaller wrapped could be like the following:

.. sourcecode:: csharp

    public class MassTransitInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, Castle.MicroKernel.SubSystems.Configuration.IConfigurationStore store)
        {
            var consumerConfig = container.Resolve<ConsumersConfiguration>(); // for configurations, like queue's address used below

            var busBatchClose = ServiceBusFactory.New(sbc =>
                {
                    sbc.UseMsmq();
                    sbc.UseMulticastSubscriptionClient();
                    sbc.ReceiveFrom(consumerConfig.BatchCloseQueue);
                    sbc.EnableMessageScope();
                    sbc.Subscribe(x => x.Handler<CloseBatchMessage>(msg =>
                        {
                            var handler = container.Resolve<IBatchCloseHandler>();
                            handler.CloseBatch(msg);
                            container.Release(handler);
                        }));
                }
            );

            container.Register(Component.For<IServiceBus>().Instance(busBatchClose).Named("BatchCloseQueueBus"));

            container.Release(consumerConfig); // irrelevant for this sample, but we need to release what we resolve.
        }
    }

AutoFac
'''''''

.. sourcecode:: csharp

    public static void main(string[] args)
    {
        var builder = new ContainerBuilder();

        // register each consumer manually
        builder.RegisterType<YourConsumer>().AsSelf();

        //or use Autofac's scanning capabilities -- SomeClass is any class in the correct assembly
        builder.RegisterAssemblyTypes(typeof(SomeClass).Assembly)
            Where(t => t.Implements<IConsumer>())
            .AsSelf();

        //now we add the bus
        builder.Register(c => ServiceBusFactory.New(sbc =>
        {
            //other configuration options

            //this will find all of the consumers in the container and
            //register them with the bus.
            sbc.Subscribe(x => x.LoadFrom(c.Resolve<ILifetimeScope>()));
        })).As<IServiceBus>()
            .SingleInstance();

        var container = builder.Build();
    }

.. note::

    We recommend that most of this type of code be placed in an Autofac Module


Ninject
'''''''

.. sourcecode:: csharp

    public static void main(string[] args) 
    {
        var kernel = new StandardKernel();
        
        // register each consumer manually
        kernel.Bind<YourConsumer>().ToSelf();
        
        //Dru is currently unaware of any scanning capability
        
        var bus = ServiceBusFactory.New(sbc =>
        {
            //other configuration options
            
            //we have to explicitly configure the subscriptions because 
            //the Ninject metadata model is not rich enough.
            sbc.Subscribe(subs =>
            {
                subs.Consumer<YourConsumer>(kernel)
            });
        });
        
        //now we add the bus
        kernel.Bind<IServiceBus>().To(bus);
    }

.. note::

    We recommend that most of this type of code be placed in an Ninject Module

.. warning::

    The Ninject container doesn't currently support the workflow that we can use with
    the other containers, and because of that the ``LoadFrom`` method that our other
    container extensions use is not supported. We filed an issue with the Ninject
    team, and the issue was closed with 'Not going to fix'. 
    https://github.com/ninject/ninject/issues/35

Unity
'''''

.. sourcecode:: csharp

	public static void main(string[] args) 
    {
		var container = new UnityContainer(); 
		
		// Lookup the types.
		// You can scan for all types that implement the .All-interface of the Consumes-class.
		var types = new TypeFinder().FindTypesWhichImplement(typeof(Consumes<>.All));
		foreach (var type in types)
		{
			var interfaceType = type.GetInterfaces().FirstOrDefault(a=> a == typeof(Consumes<>.All));
			container.RegisterType(interfaceType, type, new ContainerControlledLifetimeManager());
		}
		
		// or you can register your types directly.
		container.RegisterType<<Consumes<MessageType>.All, Type>(new ContainerControlledLifetimeManager());
		// ...

		// Register the ServiceBus.
		container.RegisterInstance<IServiceBus>(ServiceBusFactory.New(sbc =>
		{
			sbc.UseRabbitMq(c =>
			{
				// Add configation options if required.
				// Default JSON serialization is set by MassTransit.  
			});
			// Configure exchanges.
			sbc.ReceiveFrom(receiveQueue);
			sbc.Subscribe(s => s.LoadFrom(container));

			sbc.SetConcurrentConsumerLimit(concurrentConsumers);
			sbc.SetDefaultRetryLimit(retryLimit);

			// When using MSMQ as Transport you can choose to verify the DTC configuration.
			// if (verifyDTCConfiguration)
			// 		sbc.VerifyMsDtcConfiguration();

			// Configure logging.
			if (enableLogging)
				sbc.UseLog4Net();
			
			// No performance counters.
			sbc.DisablePerformanceCounters();
		}));
	}
	
Hey! Where's my container??
'''''''''''''''''''''''''''

Don't see your container here? Feel free to submit a pull request. You should easily be able to
add support by following the other containers.