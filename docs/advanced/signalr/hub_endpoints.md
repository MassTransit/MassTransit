# Hub Endpoints

The core of communication contracts between the client and server are hubs. Depending on your application and complexity you might have a few hubs as a separation of concern for your application. The backplanes work through 5 types of events **per hub**.

So this translated well into MassTransit Events:

* `All<THub>` - Invokes the method (with args) for each connection on the specified hub
* `Connection<THub>` - Invokes the method (with args) for the specific connection
* `Group<THub>` - Invokes the method (with args) for all connections belonging to the specified group
* `GroupManagement<THub>` - Adds or removes a connection to the group (on a remote server)
* `User<THub>` - Invokes the method (with args) for all connections belonging to the specific user id

So each of these Messages has a corresponding consumer, and it will get a singleton `HubLifetimeManager<THub>` through DI to perform the specific task.

MassTransit's helper extension method will create an endpoint per consumer per hub, which follows the typical recommendation of one consumer per endpoint. Because of this, the number of endpoints can grow quickly if you have many hubs. It's best to also read some [SignalR Limitations](https://docs.microsoft.com/en-us/aspnet/signalr/overview/performance/scaleout-in-signalr#limitations), to understand what can become potential bottlenecks with SignalR and your backplane. SignalR recommends re-thinking your strategy for very high throughput, real-time applications (video games).
