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
endpointConfigurator.UseRawJsonDeserializer();
```

If you want to have MT bind the endpoint to the correct topic you can do the following:

```csharp
if(endpointConfigurator is IRabbitMqReceiveEndpointConfigurator rabbit) 
{
    rabbit.Bind("your-target-topic");
}
```

## If the existing system isn't setting the content type

If the messages doesn't have a content-type set, you can tell MassTransit
what the default content type should be. Since, as the app developer, you know
the content is `application/json` you can set that manually. Then when a 
message comes in without a header, MT will select the correct one.

```csharp
endpointConfigurator.DefaultContentType = new ContentType("application/json");
endpointConfigurator.UseRawJsonDeserializer();
```
