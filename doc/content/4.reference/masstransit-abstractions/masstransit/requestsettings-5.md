---

title: RequestSettings<TSaga, TRequest, TResponse, TResponse2, TResponse3>

---

# RequestSettings\<TSaga, TRequest, TResponse, TResponse2, TResponse3\>

Namespace: MassTransit

The request settings include the address of the request handler, as well as the timeout to use
 for requests.

```csharp
public interface RequestSettings<TSaga, TRequest, TResponse, TResponse2, TResponse3> : RequestSettings<TSaga, TRequest, TResponse, TResponse2>, RequestSettings<TSaga, TRequest, TResponse>
```

#### Type Parameters

`TSaga`<br/>

`TRequest`<br/>

`TResponse`<br/>

`TResponse2`<br/>

`TResponse3`<br/>

Implements [RequestSettings\<TSaga, TRequest, TResponse, TResponse2\>](../masstransit/requestsettings-4), [RequestSettings\<TSaga, TRequest, TResponse\>](../masstransit/requestsettings-3)

## Properties

### **Completed3**

Configures the behavior of the Completed event, the same was Events are configured on
 the state machine.

```csharp
public abstract Action<IEventCorrelationConfigurator<TSaga, TResponse3>> Completed3 { get; }
```

#### Property Value

[Action\<IEventCorrelationConfigurator\<TSaga, TResponse3\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
