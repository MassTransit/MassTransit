# Integrating with other systems

Often people want to consume messages off of their broker that are coming from other
non-MassTransit systems. The below video reviews how to do this.

:video-player{src="https://www.youtube.com/watch?v=xOxSLNeN5CU"}

## Key Points

::alert{type="info"}
You must specifically configure the receive endpoint
::

Exclude this endpoint from the topology mapping. Since this endpoint is defined
by a different system, we don't need MassTransit's help here.

```csharp
endpointConfigurator.ConfigureConsumeTopology = false;
```

Add the `RawJsonSerializer` to the list of serializers supported. The serializer 
specifically looks for `application/json` versus the standard content-type used
by MassTransit which is `application/vnd.masstransit+json`.

```csharp
endpointConfigurator.UseRawJsonSerializer();
```

If you want to have MT bind the endpoint to the correct topic you can do the following:

```csharp
if(endpointConfigurator is IRabbitMqReceiveEndpointConfigurator rabbit) 
{
    rabbit.Bind("your-target-topic");
}
```

## If the existing system isn't setting the content type

If the messages doesn't have a content-type set, it will use the default serializer.
In this case, you know you want it to be json, so we can simply clear the existing ones
and then register the raw json serializer only.

```csharp
endpointConfigurator.ClearMessageDeserializers();
endpointConfigurator.UseRawJsonSerializer();
```
