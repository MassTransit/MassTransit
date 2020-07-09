# Getting Started

Getting started with MassTransit is fast and easy. This quick start guide uses RabbitMQ with .NET Core. RabbitMQ must be installed, instructions for installing RabbitMQ are included [below](#install-rabbitmq).

> The [.NET Core SDK](https://dotnet.microsoft.com/download) should be installed before continuing.

To create a service using MassTransit, create a console application via the Command Prompt.

```bash
$ mkdir GettingStarted
$ dotnet new console -o GettingStarted
```

## With In-Memory Bus

Add MassTransit package to the console application:

```bash
$ cd GettingStarted
$ dotnet add package MassTransit
```

At this point, the project should compile, but there is more work to be done. You can verify the project builds by executing:

```bash
$ dotnet run
```

### Edit Program.cs

```csharp
public class Message
{ 
    public string Text { get; set; }
}

public class Program
{
    public static async Task Main()
    {
        var bus = Bus.Factory.CreateUsingInMemory(sbc =>
        {
            sbc.ReceiveEndpoint("test_queue", ep =>
            {
                ep.Handler<Message>(context =>
                {
                    return Console.Out.WriteLineAsync($"Received: {context.Message.Text}");
                });
            });
        });

        await bus.StartAsync(); // This is important!

        await bus.Publish(new Message{Text = "Hi"});
        
        Console.WriteLine("Press any key to exit");
        await Task.Run(() => Console.ReadKey());
        
        await bus.StopAsync();
    }
}
```

Save the file, and execute _`dotnet run`_, and you should see the message *Received: Hi* displayed. If you see anything else, something went wrong. Verify your installed packages and your .NET Core setup. All the things that could possibly go wrong you should fix.

## With RabbitMQ

Add RabbitMQ for MassTransit package to the console application:

```bash
$ cd GettingStarted
$ dotnet add package MassTransit.RabbitMQ
```

If you've skipped the in-memory version, don't worry, the MassTransit package will be added as well.

If you have any errors at this point, you might want to get them resolved.

### Edit Program.cs

> You can view a working project on [GitHub](https://github.com/MassTransit/Sample-ConsoleService). There are other [samples](/learn/samples) available as well.

To get started, a bare bones *Program.cs* is shown below.

```csharp
public class Message
{ 
    public string Text { get; set; }
}

public class Program
{
    public static async Task Main()
    {
        var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
        {
            sbc.Host("rabbitmq://localhost");

            sbc.ReceiveEndpoint("test_queue", ep =>
            {
                ep.Handler<Message>(context =>
                {
                    return Console.Out.WriteLineAsync($"Received: {context.Message.Text}");
                });
            });
        });

        await bus.StartAsync(); // This is important!

        await bus.Publish(new Message{Text = "Hi"});
        
        Console.WriteLine("Press any key to exit");
        await Task.Run(() => Console.ReadKey());
        
        await bus.StopAsync();
    }
}
```

Save the file, and execute _dotnet run_, and you should see the message *Received: Hi* displayed. If you see anything else, something went wrong. Verify your RabbitMQ installation and your .NET Core setup, basically all the things that could possibly go wrong you should fix.

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

### What is this doing?

If we are going to create a messaging system, we need to create a message. `Message` is a .NET class that will represent our message. Notice that it's just a Plain Old CLR Object (or POCO).

Next up, we need a program to run our code. Here we have a standard issue command line `Main` method. To setup the bus we start with the static class `Bus` and work off of the `Factory` extension point. From there we call the `CreateUsingRabbitMQ` method to setup a RabbitMQ bus instance. This method takes a lambda whose first and only argument is a class that will let you configure every aspect of the bus.

One of your first decisions is going to be "What transport do I want to run on?" Here we have chosen RabbitMQ (`Bus.Factory.CreateUsingRabbitMQ()`) because its the defacto transport choice for MassTransit.

After that we need to configure the RabbitMQ host settings `sbc.Host()`. The first argument sets the machine name and the virtual directory to connect to. After that you have a lambda that you can use to tweak any of the other settings that you want. Since no additional configuration is specified, the default username and password (guest/guest) is being used.

Now that we have a host to listen on, we can configure some receiving endpoints `sbc.ReceiveEndpoint`. We specifiy the queue we want to listen on and a lambda to register each handler that we want to use.

Lastly, in the configuration, we have the `Handler<Message>` method which subscribes a handler for the message type `Message` and takes an `async` lambda (oh yeah baby TPL) that is given a context class to process. Here we access the message by traversing `context.Message` and then writing to the console the text of the message.

And now we have a bus instance that is fully configured and can start processing messages. We can grab the `busControl` that we created and call `StartAsync` on it to get everything rolling. We again `await` on the result and now we can go.

::: warning IMPORTANT
You *must* start the bus, otherwise you will get issues with sending and receiving messages. There is no "send-only" bus with MassTransit.
:::

We can call the `Publish` method on the `busControl` and we should see our console write the output.
