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
            
            //or use StructureMap's excellent scanning capabilities
        });
        
        var bus = ServiceBusFactory.New(sbc =>
        {
            //other configuration options
            
            //this will find all of the consumers in the container and 
            //register them with the bus.
            sbc.LoadFrom(container);
        });
        
        //now we add the bus
        container.Inject<IServiceBus>(bus);
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
        container.Register(Component.For<IConsumer>().ImplementedBy<YourConsumer>);
        
        //or use Windsor's excellent scanning capabilities
        container.Register(AllTypes.FromThisAssembly().BasedOn<IConsumer>());
        
        var bus = ServiceBusFactory.New(sbc =>
        {
            //other configuration options
            
            //this will find all of the consumers in the container and 
            //register them with the bus.
            sbc.LoadFrom(container);
        });
        
        //now we add the bus
        container.Register(Component.For<IServiceBus>().Instance(bus));
    }

.. note::

    We recommend that most of this type of code be placed in an IWindsorInstaller

AutoFac
'''''''

Coming soon. Feel free to write it up.

Unity
'''''

Coming soon. Feel free to write it up.

Ninject
'''''''

Coming soon. Feel free to write it up.

Hey! Where's my container??
'''''''''''''''''''''''''''

Don't see your container here? Feel free to submit a pull request. You should easily be able to
add support by following the other containers.