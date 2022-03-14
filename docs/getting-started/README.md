# Getting Started

Getting started with MassTransit is fast and easy.

- The [.NET 5 SDK](https://dotnet.microsoft.com/download) should be installed before continuing.
- The source for this sample is available [on GitHub](https://github.com/MassTransit/Sample-GettingStarted).

::: tip
MassTransit includes project and item templates simplifying the creation of new projects. Install the templates by executing `dotnet new -i MassTransit.Templates` at the console. A video introducing the templates is available on [YouTube](https://youtu.be/nYKq61-DFBQ).
:::

To create a service using MassTransit, create a worker via the Command Prompt.

```bash
$ mkdir GettingStarted
$ dotnet new worker -n GettingStarted
```

## Using the InMemory Transport

Add MassTransit package to the console application:

```bash
$ cd GettingStarted
$ dotnet add package MassTransit
```

At this point, the project should compile, but there is more work to be done. You can verify the project builds by executing:

```bash
$ dotnet run
```

You should see something similar to the output below (press Control+C to exit).

```
Building...
info: GettingStarted.Worker[0]
      Worker running at: 03/24/2021 11:38:29 -05:00
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
info: Microsoft.Hosting.Lifetime[0]
      Hosting environment: Development
info: Microsoft.Hosting.Lifetime[0]
      Content root path: /Users/chris/Garbage/start/GettingStarted
info: GettingStarted.Worker[0]
      Worker running at: 03/24/2021 11:38:30 -05:00
```

### Edit Program.cs

```csharp
using MassTransit;
```

```csharp {5-19}
public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureServices((hostContext, services) =>
        {
            services.AddMassTransit(x =>
            {
                x.AddConsumer<MessageConsumer>();

                x.UsingInMemory((context, cfg) =>
                {
                    cfg.ConfigureEndpoints(context);
                });
            });

            services.AddOptions<MassTransitHostOptions>()
                .Configure(options =>
                {
                    options.WaitUntilStarted = true; 
                });
            
            services.AddHostedService<Worker>();
        });
```

### Add a Consumer

Create a new class file `MessageConsumer.cs` in the project:

```csharp
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace GettingStarted
{
    public class Message
    {
        public string Text { get; set; }
    }

    public class MessageConsumer :
        IConsumer<Message>
    {
        readonly ILogger<MessageConsumer> _logger;

        public MessageConsumer(ILogger<MessageConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<Message> context)
        {
            _logger.LogInformation("Received Text: {Text}", context.Message.Text);

            return Task.CompletedTask;
        }
    }
}
```

### Update the Worker

```csharp
public class Worker : BackgroundService
{
    readonly IBus _bus;

    public Worker(IBus bus)
    {
        _bus = bus;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await _bus.Publish(new Message {Text = $"The time is {DateTimeOffset.Now}"});

            await Task.Delay(1000, stoppingToken);
        }
    }
}
```

### Run the project

```bash
$ dotnet run
```

The output should have changed to show the message consumer generating the output (again, press Control+C to exit).

``` {2-5,12-15}
Building...
info: MassTransit[0]
      Configured endpoint Message, Consumer: GettingStarted.MessageConsumer
info: MassTransit[0]
      Bus started: loopback://localhost/
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
info: Microsoft.Hosting.Lifetime[0]
      Hosting environment: Development
info: Microsoft.Hosting.Lifetime[0]
      Content root path: /Users/chris/Garbage/start/GettingStarted
info: GettingStarted.MessageConsumer[0]
      Received Text: The time is 3/24/2021 12:02:01 PM -05:00
info: GettingStarted.MessageConsumer[0]
      Received Text: The time is 3/24/2021 12:02:02 PM -05:00
```

At this point, the consumer is configured on the bus and messages are published to the consumer.

## With RabbitMQ

::: tip
To continue from this point, RabbitMQ must be installed. If you need to install RabbitMQ, refer to the [Install RabbitMQ](#install-rabbitmq) section below.
:::

Add the _MassTransit.RabbitMQ_ package to the project.

```bash
$ dotnet add package MassTransit.RabbitMQ
```

### Edit Program.cs

Change `UsingInMemory` to `UsingRabbitMq`.

```csharp {9}
public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureServices((hostContext, services) =>
        {
            services.AddMassTransit(x =>
            {
                x.AddConsumer<MessageConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.ConfigureEndpoints(context);
                });
            });

            services.AddOptions<MassTransitHostOptions>()
                .Configure(options =>
                {
                    options.WaitUntilStarted = true; 
                });

            services.AddHostedService<Worker>();
        });
```

### Run the project

```bash
$ dotnet run
```

The output should have changed to show the message consumer generating the output (again, press Control+C to exit). Notice that the bus address now starts with `rabbitmq`.

``` {11}
Building...
info: MassTransit[0]
      Configured endpoint Message, Consumer: GettingStarted.MessageConsumer
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
info: Microsoft.Hosting.Lifetime[0]
      Hosting environment: Development
info: Microsoft.Hosting.Lifetime[0]
      Content root path: /Users/chris/Garbage/start/GettingStarted
info: MassTransit[0]
      Bus started: rabbitmq://localhost/
info: GettingStarted.MessageConsumer[0]
      Received Text: The time is 3/24/2021 12:11:10 PM -05:00
```

At this point the service is connecting to RabbbitMQ on `localhost` and publishing messages which are received by the consumer.

## With Azure Service Bus

::: tip
To continue from this point, you must have a valid Azure subscription with an Azure Service Bus namespace. A shared access policy with _Manage_ permissions is required to use MassTransit with Azure Service Bus.
:::

Add the _MassTransit.Azure.ServiceBus.Core_ package to the project.

```bash
$ dotnet add package MassTransit.Azure.ServiceBus.Core
```

### Edit Program.cs

Change `UsingInMemory` to `UsingAzureServiceBus`.

```csharp {9}
public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureServices((hostContext, services) =>
        {
            services.AddMassTransit(x =>
            {
                x.AddConsumer<MessageConsumer>();

                x.UsingAzureServiceBus((context, cfg) =>
                {
                    var connectionString = "your connection string";
                    cfg.Host(connectionString);

                    cfg.ConfigureEndpoints(context);
                });
            });

            services.AddOptions<MassTransitHostOptions>()
                .Configure(options =>
                {
                    options.WaitUntilStarted = true; 
                });

            services.AddHostedService<Worker>();
        });
```

### Run the project

```bash
$ dotnet run
```

The output should have changed to show the message consumer generating the output (again, press Control+C to exit). Notice that the bus address now starts with `sb`.

``` {11}
Building...
info: MassTransit[0]
      Configured endpoint Message, Consumer: GettingStarted.MessageConsumer
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
info: Microsoft.Hosting.Lifetime[0]
      Hosting environment: Development
info: Microsoft.Hosting.Lifetime[0]
      Content root path: /Users/chris/Garbage/start/GettingStarted
info: MassTransit[0]
      Bus started: sb://your-service-bus-namespace/
info: GettingStarted.MessageConsumer[0]
      Received Text: The time is 3/24/2021 12:11:10 PM -05:00
```

At this point, the service is connecting to Azure Service Bus and publishing messages which are received by the consumer.


### What is this doing?

If we are going to create a messaging system, we need to create a message. `Message` is a .NET class that will represent our message. Notice that it's just a Plain Old CLR Object (or POCO).

Next up, the `AddMassTransit` extension is used to configure the bus in the container. The `UsingInMemory` (and `UsingRabbitMq`) method specifies the transport to use for the bus. Each transport has its own `UsingXxx` method.

The consumer is added, using `AddConsumer`. This adds the consumer to the container as scoped.

The `ConfigureEndpoints` method is used to automatically configure the receive endpoints on the bus. In this case, a single receive endpoint will be created for the `MessageConsumer`.

The `AddOptions` call is adding options to configure the MassTransit hosted service. In this case, we want the worker to start _after_ the bus has successfully started. By default, the hosted service StartAsync method does not wait until the bus is connected/healthy.

Lastly, the `Worker` is updated to publish a message every second to the bus, which is subsequently consumed by the consumer.


### Install RabbitMQ

RabbitMQ can be installed several different ways, depending upon your operating system and installed software.

#### Docker

The easiest by far is using Docker, which can be started as shown below. This will download and run a preconfigured Docker image, maintained by MassTransit, including the delayed exchange plug-in, as well as the Management interface enabled.

```bash
$ docker run -p 15672:15672 -p 5672:5672 masstransit/rabbitmq
```

#### Homebrew (Mac OS X)

If you are using a Mac, RabbitMQ can be installed using [Homebrew](https://brew.sh/) by typing `brew install rabbitmq`. This installs the management plug-in automatically. Once installed, type `brew services start rabbitmq` and accept the prompts to enable network ports.

#### To install RabbitMQ manually:

 1. **Install Erlang** using the [installer](http://www.erlang.org/download.html). (Next -> Next ...)
 2. **Install RabbitMQ** using the [installer](http://www.rabbitmq.com/download.html). (Next -> Next ...) You now have a RabbitMQ broker (look in `services.msc` for it) that you can [log into](http://localhost:15672/#/) using `guest`, `guest`. You can see message rates, routings and active consumers using this interface. 
 
##### You need to add the management interface before you can login.

1. First, from an elevated command prompt, change directory to the sbin folder within the RabbitMQ Server installation directory e.g. `%PROGRAMFILES%\RabbitMQ Server\rabbitmq_server_3.5.3\sbin\`

2. Next, run the following command to enable the rabbitmq management plugin: `rabbitmq-plugins enable rabbitmq_management`

