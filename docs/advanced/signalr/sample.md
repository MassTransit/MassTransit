# Sample

We've included a sample ASP.NET Core project, and backend console app to show interop with the backplane. The only thing needed is RabbitMQ. I'd recommend using their [docker image](https://store.docker.com/community/images/library/rabbitmq) to spin up the broker.

## Sample-SignalR

You can view the [MassTransit Sample here](https://github.com/MassTransit/Sample-SignalR). The sample was based off of [microsofts chat sample](https://github.com/aspnet/SignalR-samples/tree/master/ChatSample), which is nearly identical to the [tutorial here](https://docs.microsoft.com/en-us/aspnet/core/tutorials/signalr?view=aspnetcore-2.2&tabs=visual-studio), except the only different is it's stripped down to the bare minimum (no razor Pages, bootstrap or jquery libraries).

The other difference is the javascript client callback method name is "ReceiveMessage" versus "broadcastMessage", but both samples are nearly the same. and the hub route is /chat versus /chatHub.

The other addition we added is in the Properties/launchSettings.json, which lets us start 2 profiles on different ports. Then helps simulate horizontal scaling.

### Mvc Sample

You can simulate scaleout by running the two profiles.

```
> cd (your cloned Sample-SignalR)\src\SampleSignalR.Mvc
> dotnet run --launch-profile sample1
> dotnet run --launch-profile sample2
```

Now in two browser tabs, open up in each:
http://localhost:5100
http://localhost:5200

Then you can type a message in each, and see them show up in the other. The backplane works!!

## Console Sample

If you have some backend services (console apps, or Mt Topshelf consumers), you might want to notify users/groups of things that have happened in realtime. You can do this by running this console app.

```
> cd (your cloned Sample-SignalR)\src\SampleSignalR.Service
> dotnet run
```

An type in a message to broadcast to all connections. You will see the message in your browsers chat messages
