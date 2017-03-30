# Under the hood

MassTransit hides much of the complexity, making it easier for developers to use messaging in their applications. However, when issues arise, it's important to understand how things work under the hood to effectively troubleshoot and resolve the issues.

## Setup

First, there are two parts to this discovery, the first is purely about the framework (MassTransit) features and the second goes into how the various transports are leveraged to support the framework features.

[The Second](default-topology.md) article has nice graphics too!

Consider the following message, it's a command telling a service to process a file.

```csharp
namespace BigBusiness.Contracts
{
  interface ProcessFile
  {
    Guid FileId { get; }
    Uri Location { get; }
  }
}
```

In code, the developer would create a consumer and a receive endpoint that consumes that command.

```csharp
class ProcessFileConsumer :
  IConsumer<ProcessFile>
{
}
```

The receive endpoint in this example will use `service-queue` for the queue name.

```csharp
cfg.ReceiveEndpoint("service-queue", e =>
{
  e.Consumer<ProcessFileConsumer>();
});
```

MassTransit uses the definition of the message contract (`ProcessFile`), combined with the receive endpoint queue name (`service-queue`) to build out the topology on the broker (the details of which are covered in the [second](default-topology.md) part).

## Queues

 - Each application you write should use a unique queue name.
 - If you run multiple copies  of your consumer service, they would listen to the same queue (as they are copies).
   This would mean you have multiple applications receiving messages from `service-queue` 
   This would result in a 'competing consumer' scenario, which is commonly used for load-balancing across servers.  (Which is what you want if you run same service multiple times)
 - If there is an exception from your consumer, the message will be sent to `service-queue-error` queue.
 - If a message is received in a queue that the consumer does not know how to handle, the message will be sent 
   to `service-queue-skipped` queue.

## Design Benefits

 - Any application can listen to any message and that will not affect any other application that may or may not be listening for that message
 - Any application(s) that bind a group of messages to the same queue will result in the competing consumer pattern.
 - You do not have to concern yourself with anything but what message type to produce and what message type to consume.

## Faq

* How many messages at a time will be simultaniously processed?
    * Each endpoint you create represents 1 queue.  That queue can receive any number of different message types (based on what you subscribe to it)
    * The configuration of each endpoint you can set the number of consumers with a call to `PrefetchCount(x)`.  
    * This is the total number of consumers for all message types sent to this queue.
    * In MT2, you had to add ?prefetch=X to the Rabbit URL. This is handled automatically now.


* Can I have a set number of consumers per message type?
	* Yes. This uses middleware. 
   
    `x.Consumer(new AutofacConsumerFactory<…>(), p => p.UseConcurrencyLimit(1));  x.PrefetchCount=16;`
    
     PrefetchCount should be relatively high, a multiple of your concurrency limit for all message types so that RabbitMQ doesn’t choke delivery messages due to network delays. Always have a queue ready to receive the message.


* When my consumer is not running, I do not want the messages to wait in the queue.  How can I do this?
    * There are two ways.  Note that each of these imply you would never use a 'competing consumer' pattern, so make sure that is the case.
		1. Set `PurgeOnStartup=true` in the endpoint configuration. When the bus starts, it will empty the queue of all messages.
		2. Set `AutoDelete=true` in the endpoint configuration. This causes the queue to be removed when your application stops.


* How are Retrys handled?
    * This is handled by [middleware](middleware.md). Each endpoint has a [retry policy](retry.md). 


* Can I have a different retry policy per each message type?  
    * No. This is set at an endpoint level. You would have to have a specific queue per consumer to achieve this.  


