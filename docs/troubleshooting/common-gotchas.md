# Common Mistakes

Over the years, there are certain concepts that can be confusing and lead to questions for developers new to MassTransit (or message-based asynchronous programming). A few of the common mistakes, issues, and gotchas are described below.

::: danger Have you started the bus?
Seriously, this is so common it's worth repeating at the top of every page. If you are seeing messages not being consumed, or responses timing out on request, or anything that feels weird, make sure you are calling `Start` or `StartAsync` on the `IBusControl`.
:::

### Sharing a queue

> While a common mistake in MassTransit 2.x, the new receive endpoint syntax of MassTransit 3 should make it easier to recognize that queue names should not be shared.

Each receive endpoint needs to have a unique queue name! If multiple receive endpoints are created,
each should have a different queue name so that messages are not skipped.

If two receive endpoints share the same queue name, yet have different consumers subscribed, messages
which are received by one endpoint but meant for the other will be moved to the _skipped_ queue. It
would be like sharing a mailbox with your neighbor, sometimes you get all the mail, sometimes they
get all the mail.

### Send/Publish Only

When creating a bus instance only to send or publish messages, it must be started. Failure to start the bus can lead to some strange side effects. Every bus, even ones without receive endpoints, must be started (and eventually stopped).

### How do I load balance consumers across machines?

To load balance consumers, the process with the receive endpoint can be hosted on multiple servers.
As long as each receive endpoint has the same consumers registered, the messages will be received
by the first available consumer across all of the machines.

#### What links two bus instances together?

This is a common question. The binding element, really is the
message contract. If you want message A, then you subscribe to
message A. The internals of MT wires it all together.

### Why aren't queue / message priorities supported?

Message Priorities are used to allow a message to jump to the front
of the line. When people ask for this feature they usually have multiple
types of messages all being delivered to the same queue. The problem
is that each message has a different SLA (usually the one with the
shorter time window is the one getting the priority flag). The problem
is that w/o priorities the important message gets stuck behind the
less important/urgent ones.

The solution is to stop sharing a single queue, and instead establish
a second queue. In MassTransit you would establish a second instance
of IServiceBus and have it subscribe to the important/urgent
message. Now you have two queues, one for the important things and one
for the less urgent things. This helps with monitoring queue depths,
error rates, etc. By placing each IServiceBus in its own Topshelf host
/ process you further enhance each bus's ability to process messages, and
isolate issues / downtime.

#### Request client throws a timeout exception

MassTransit uses a temporary non-durable queue and has a consumer to handle responses. This temporary queue only get configured and created when you _start the bus_. If you forget to start the bus in your application code, the request client will fail with a timeout, waiting for a response.

#### Reading

http://lostechies.com/jimmybogard/2010/11/18/queues-are-still-queues/

### I want to know if another bus is subscribed to my message.

> So, if you try to program this way, you're going to have a bad time. ;)

Knowing that you have a subscriber is not the concern of your application.
It is something the system architect should know, but not the application.
Most likely, we just need to introduce all of the states in our protocol
more explicitly, by using a Saga.
