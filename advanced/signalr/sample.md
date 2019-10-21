# Sample

We've included a sample ASP.NET Core project, and backend console app to show interop with the backplane. The only thing needed is RabbitMQ. I'd recommend using their [docker image](https://store.docker.com/community/images/library/rabbitmq) to spin up the broker.

## MassTransit.SignalR.Sample

This [chat sample](https://github.com/aspnet/SignalR-samples/tree/master/ChatSample) was used, which is nearly identical to the [tutorial here](https://docs.microsoft.com/en-us/aspnet/core/tutorials/signalr?view=aspnetcore-2.2&tabs=visual-studio), except the only different is it's stripped down to the bare minimum (no razor Pages, bootstrap or jquery libraries).

The other difference is the javascript client callback method name is "ReceiveMessage" versus "broadcastMessage", but both samples are nearly the same. and the hub route is /chat versus /chatHub.

The other addition we added is in the Properties/launchSettings.json, which lets us start 2 profiles on different ports. Then helps simulate horizontal scaling.

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