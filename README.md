MassTransit
=======

MassTransit is a _free, open-source_ distributed application framework for .NET. MassTransit makes it easy to create applications and services that leverage message-based, loosely-coupled asynchronous communication for higher availability, reliability, and scalability.

![Mass Transit](https://raw.githubusercontent.com/MassTransit/MassTransit/develop/doc/source/mt-logo.png "Mass Transit")

MassTransit is Apache 2.0 licensed.

![master](https://ci.appveyor.com/api/projects/status/hox8dhh5eyy7jsf4/branch/master?svg=true)
![develop](https://ci.appveyor.com/api/projects/status/hox8dhh5eyy7jsf4/branch/develop?svg=true)

## Getting started with MassTransit

In order to get started with MassTransit, you can have a look at the
documentation, which is located at [http://masstransit-project.com/MassTransit](http://masstransit-project.com/MassTransit).

### Simplest possible thing:

`install-package MassTransit.RabbitMq`

and then:

```csharp
// Message Definition
class MyMessage
{
    public string Value { get; set; }
}

// Code Snippet for Console Application 
var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
{
    var host = sbc.Host(new Uri("rabbitmq://localhost/"), h =>
    {
        h.Username("guest");
        h.Password("guest");
    });

    sbc.ReceiveEndpoint(host, "my_queue", endpoint =>
    {
        endpoint.Handler<MyMessage>(async context =>
        {
            await Console.Out.WriteLineAsync($"Received: {context.Message.Value}");
        });
    });
});

using(bus.Start())
{
    bus.Publish(new MyMessage{Value = "Hello, World."});

    Console.ReadLine();
}
```

You will also need to set up RabbitMQ; 

 1. **Install Erlang** using the [installer](http://www.erlang.org/download.html). (Next -> Next ...)
 2. **Install RabbitMQ** using the [installer](http://www.rabbitmq.com/download.html). (Next -> Next ...) You now have a RabbitMQ broker (look in `services.msc` for it) that you can [log into](http://localhost:15672/#/) using `guest`, `guest`. You can see message rates, routings and active consumers using this interface. 
 
**Please note** 

You need to add the management interface before you can login. First, from an elevated command prompt, change directory to the sbin folder within the RabbitMQ Server installation directory e.g. `%PROGRAMFILES%\RabbitMQ Server\rabbitmq_server_3.5.3\sbin\`

Next, run the following command to enable the rabbitmq management plugin:

`rabbitmq-plugins.bat enable rabbitmq_management`

### Downloads

Download from NuGet 'MassTransit' [Search NuGet for MassTransit](http://nuget.org/packages?q=masstransit)

Download the continuously integrated Nuget packages from [AppVeyor](https://ci.appveyor.com/project/phatboyg/masstransit/build/artifacts).

### Supported transports

We support RabbitMQ and Azure Service Bus message brokers.

## Mailing list

[MassTransit Discuss](http://groups.google.com/group/masstransit-discuss)

## Gitter Chat 

While attendance is pretty light, there is a Gitter chat room available:

[![Join the chat at https://gitter.im/MassTransit/MassTransit](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/MassTransit/MassTransit?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

## GitHub Issues

**Pay attention**

Please do not open an issue on github, unless you have spotted an actual bug in MassTransit. 
If you are unsure, ask on the mailing list, and if we confirm it's a bug, we'll ask you to create the issue. 
Issues are not the place for questions, and they'll likely be closed.

This policy is in place to avoid bugs being drowned out in a pile of sensible suggestions for future 
enhancements and calls for help from people who forget to check back if they get it and so on.

## Building from Source

 1. Clone the source down to your machine. 
   `git clone git://github.com/MassTransit/MassTransit.git`
 1. Run `build.bat`

## Contributing

 1. `git config --global core.autocrlf false`
 1. Hack!
 1. Make a pull request.

## Builds

MassTransit is built on [AppVeyor](https://ci.appveyor.com/project/phatboyg/masstransit)
 
# REQUIREMENTS
* .Net 4.5.2

# CREDITS
Logo Design by [The Agile Badger](http://www.theagilebadger.com)
