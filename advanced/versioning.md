# Versioning messages

Versioning of messages is going to happen, services evolve and requirements change.

## Versioning existing message contracts

Consider a command to fetch and cache a local copy of an image from a remote system.

```csharp
public interface FetchRemoteImage
{
    Guid CommandId { get; }
    DateTime Timestamp { get; }
    Uri ImageSource { get; }
    string LocalCacheKey { get; }
}
```

After the initial deployment, a requirement is added to resize the image to a
maximum dimension before saving it to the cache. The new message contract
includes the additional property specifying the dimension.

```csharp
public interface FetchRemoteImage
{
    Guid CommandId { get; }
    DateTime Timestamp { get; }
    Uri ImageSource { get; }
    string LocalCacheKey { get; }
    int? MaximumDimension { get; }
}
```

By making the *int* value nullable, commands that are submitted using the
original contract can still be accepted as the missing value does not break the
new contract. If the value was added as a regular *int*, it would be assigned a
default value of zero, which may not convey the right information. String
values can also be added as they will be *null* if the value is not present in
the serialized message. The consumer just needs to check if the value is
present and process it accordingly.

# Versioning existing events

Consider an event to notify that an image has been cached is now available.

```csharp
public interface RemoteImageCached
{
    Guid EventId { get; }
    DateTime Timestamp { get; }
    Guid InitiatingCommandId { get; }
    Uri ImageSource { get; }
    string LocalCacheKey { get; }
}
```

An application will publish the event using an implementation of the class, as shown below.

```csharp
class RemoteImageCachedEvent :
    RemoteImageCached
{
    Guid EventId { get; set; }
    DateTime Timestamp { get; set; }
    Guid InitiatingCommandId { get; set; }
    Uri ImageSource { get; set; }
    string LocalCacheKey { get; set; }
}
```

The class implements the event interface, and when published, is delivered to
consumers that are subscribed to the *RemoteImageCached* event interface.
MassTransit dynamically creates a backing class for the interface, and
populates the properties with the values from the serialized message.

> Note that you cannot dynamically cast the *RemoteImageCached* interface in
> the consumer to the RemoteImageCachedEvent, as the actual class is not
> deserialized. This can be confusing, but is intentional to prevent classes (and
> the behavior that comes along with it) from being serialized and deserialized.

As the event evolves, additional event contracts can be defined that include
additional information without modifying the original contract. For example.

```csharp
public interface RemoteImageCachedV2
{
    Guid EventId { get; }
    DateTime Timestamp { get; }
    Guid InitiatingCommandId { get; }
    Uri ImageSource { get; }

    // the string is changed from LocalCacheKey to a full URI
    Uri LocalImageAddress { get; }
}
```

The event class is then modified to include the additional property, while
still implementing the previous interface.

```csharp
class RemoteImageCachedEvent :
    RemoteImageCached,
    RemoteImageCachedV2
{
    Guid EventId { get; set; }
    DateTime Timestamp { get; set; }
    Guid InitiatingCommandId { get; set; }
    Uri ImageSource { get; set; }
    string LocalCacheKey { get; set; }
    Uri LocalImageAddress { get; set; }
}
```

When the event class is published now, both interfaces are available in the
message. When a consumer subscribes to one of the interfaces, that consumer
will receive a copy of the message. It is important that both interfaces are
not consumed in the same context, as duplicates will be received. If a service
is updated, it should use the new contract.

> Note that ownership of the contract belongs to the event publisher, not the
> event observer/subscriber. And contracts should not be shared between event
> producers as this can create some extensive leakage of multiple events making
> it difficult to consume unique events.

As mentioned above, depending upon the interface type subscribed, a dynamic
backing class is created by MassTransit. Therefore, if a consumer subscribes to
RemoteImageCached, it is not possible to cast the message to
RemoteImageCachedV2, as the dynamic implementation does not support that
interface.

>    It should be noted, however, that on the IConsumeContext interface, there
>    is a method to TryGetContext<T> method, which can be used to attempt to
>    deserialize the message as type T. So it is possible to check if the
>    message also implements the new version of the interface and not process as
>    the original version knowing that the new version will be processed on the
>    same message consumption if both types are subscribed.

The message is a single message on the wire, but the available/known types are
captured in the message headers so that types can be deserialized from the
message body.

A lot of flexibility and power, it's up to the application developer to ensure
that it is used in a way that ensures application evolution over time without
requiring forklift/switchover upgrades due to breaking message changes.

