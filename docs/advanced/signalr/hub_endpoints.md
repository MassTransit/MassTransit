# Hub Endpoints

The core of communication contracts between the client and server are hubs. Depending on your application and complexity you might have a few hubs as a separation of concern for your application. The backplanes work through 5 types of events **per hub**.

So this translated well into MassTransit Events:

* `All<THub>` - Invokes the method (with args) for each connection on the specified hub
  * The `ExcludedConnectionIds` property allows exclusion of specific connection ids
* `Connection<THub>` - Invokes the method (with args) for the specific connection
  * The `ConnectionId` indicates which connection id to send the message to. If no active connection was found for this id, no exception will be thrown and the message will be completed without further processing.
* `Group<THub>` - Invokes the method (with args) for all connections belonging to the specified group
  * The `GroupName` property indicates the name of the target group
  * The `ExcludedConnectionIds` property allows exclusion of specific connection ids
* `GroupManagement<THub>` - Adds or removes a connection to the group (on a remote server)
* `User<THub>` - Invokes the method (with args) for all connections belonging to the specific user id
  * The `UserId` property indicates the id of the user to be targeted. Note that although similar, this differs from `Connection<THub>` since the user id is generally a specific identifier given to a user, whereas the connection id is usually a random identifier assigned to each connection.

All event types contain a property `Messages` which transports the payload as an `IReadOnlyDictionary<string, byte[]>`.

So each of these Messages has a corresponding consumer, and it will get a `HubLifetimeManager<THub>` through DI to perform the specific task. Messages sent through these endpoints will be published on your configured message broker, and once consumed, will be sent to your SignalR clients according to the configured message type.

In case an exception occurs while sending a message through the SignalR connection, an exception will be logged, but the message itself will still be marked as completed.

MassTransit's helper extension method will create an endpoint per consumer per hub, which follows the typical recommendation of one consumer per endpoint. Because of this, the number of endpoints can grow quickly if you have many hubs. It's best to also read some [SignalR Limitations](https://docs.microsoft.com/en-us/aspnet/signalr/overview/performance/scaleout-in-signalr#limitations), to understand what can become potential bottlenecks with SignalR and your backplane. SignalR recommends re-thinking your strategy for very high throughput, real-time applications (video games).
