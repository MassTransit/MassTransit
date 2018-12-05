# Sample

We've included a sample ASP.NET Core project, and backend console app to show interop with the backplane. The only thing needed is RabbitMQ. I'd recommend using their [docker image](https://store.docker.com/community/images/library/rabbitmq) to spin up the broker.

## MassTransit.SignalR.Sample

A near identical copy of the [ASP.NET Core SignalR Tutorial](https://docs.microsoft.com/en-us/aspnet/core/tutorials/signalr?view=aspnetcore-2.1&tabs=visual-studio), but with the MassTransit backplane registration included. Plus two run profile to simulate scaleout.

### Dotnet Run

You can simulate scaleout by running the two profiles.

```cmd
> cd (your path to MassTransit clone)\src\MassTransit.SignalR.Sample
> dotnet run --launch-profile sample1
> dotnet run --launch-profile sample2
```

Now in two browser tabs, open up in each:
http://localhost:5100
http://localhost:5200

Then you can type a message in each, and see them show up in the other. The backplane works!!

## MassTransit.SignalR.SampleConsole

If you have some backend services (console apps, or Mt Topshelf consumers), you might want to notify users/groups of things that have happened in realtime. You can do this by running this console app.

```cmd
> cd (your path to MassTransit clone)\src\MassTransit.SignalR.SampleConsole
> dotnet run
```

An type in a message to broadcast to all connections. You will see the message in your browsers chat messages