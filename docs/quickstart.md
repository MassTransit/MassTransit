# Getting started

To get started using MassTransit, create a console application using:

```
mkdir GettingStarted
dotnet new console -o GettingStarted
```

Add some references to the newly created console application:

```
cd GettingStarted
dotnet add package MassTransit.RabbitMQ
```

At this point, the project should compile, but there is more work to be done.

## Show me the code!

> You can view a working project on [GitHub](https://github.com/MassTransit/Sample-ConsoleService).

A bare bones `Program.cs` is shown below, just to get started.

```csharp
public class YourMessage
{ 
    public string Text { get; set; }
}

public class Program
{
    public static void Main()
    {
        var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
        {
            var host = sbc.Host(new Uri("rabbitmq://localhost"), h =>
            {
                h.Username("guest");
                h.Password("guest");
            });

            sbc.ReceiveEndpoint(host, "test_queue", ep =>
            {
                ep.Handler<YourMessage>(context =>
                {
                    return Console.Out.WriteLineAsync($"Received: {context.Message.Text}");
                });
            });
        });

        bus.Start(); // This is important!

        bus.Publish(new YourMessage{Text = "Hi"});
        
        Console.WriteLine("Press any key to exit");
        Console.ReadKey();
        
        bus.Stop();
    }
}
```

### Install RabbitMQ

You will also need to set up RabbitMQ; 

 1. **Install Erlang** using the [installer](http://www.erlang.org/download.html). (Next -> Next ...)
 2. **Install RabbitMQ** using the [installer](http://www.rabbitmq.com/download.html). (Next -> Next ...) You now have a RabbitMQ broker (look in `services.msc` for it) that you can [log into](http://localhost:15672/#/) using `guest`, `guest`. You can see message rates, routings and active consumers using this interface. 
 
#### You need to add the management interface before you can login.

1. First, from an elevated command prompt, change directory to the sbin folder within the RabbitMQ Server installation directory e.g. `%PROGRAMFILES%\RabbitMQ Server\rabbitmq_server_3.5.3\sbin\`

2. Next, run the following command to enable the rabbitmq management plugin: `rabbitmq-plugins.bat enable rabbitmq_management`


### What is this doing?

If we are going to create a messaging system, we need to create a message. `YourMessage` is a .NET class that will represent our message. Notice that it's just a Plain Old CLR Object (or POCO).

Next up, we need a program to run our code. Here we have a standard issue command line `Main` method. To setup the bus we start with the static class `Bus` and work off of the `Factory` extension point. From there we call the `CreateUsingRabbitMQ` method to setup a RabbitMQ bus instance. This method takes a lambda whose first and only argument is a class that will let you configure every aspect of the bus.

One of your first decisions is going to be "What transport do I want to run on?" Here we have chosen RabbitMQ (`Bus.Factory.CreateUsingRabbitMQ()`) because its the defacto transport choice for MassTransit.

After that we need to configure the RabbitMQ host settings `sbc.Host()`. The first argument sets the machine name and the virtual directory to connect to. After that you have a lambda that you can use to tweak any of the other settings that you want. Here we can see it setting the username and password.

Now that we have a host to listen on, we can configure some receiving endpoints `sbc.ReceiveEndpoint`. We pass in the host connection to listen on, then which queue do we want to listen on, and finally a lambda to register each handler that we want to use.

Lastly, in the configuration, we have the `Handler<YourMessage>` method which subscribes a handler for the message type `YourMessage` and takes an `async` lambda (oh yeah baby TPL) that is given a context class to process. Here we access the message by traversing `context.Message` and then writing to the console the text of the message.

And now we have a bus instance that is fully configured and can start processing messages. We can grab the `busControl` that we created and call `Start` on it to get everything rolling. We again `await` on the result and now we can go.

> **Important** you need to start the bus, otherwise you will get issues with sending and receiving messages. There is no "send-only" bus with MassTransit.

We can call the `Publish` method on the `busControl` and we should see our console write the output.
