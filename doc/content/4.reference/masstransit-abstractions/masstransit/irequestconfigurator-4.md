---

title: IRequestConfigurator<TInstance, TRequest, TResponse, TResponse2>

---

# IRequestConfigurator\<TInstance, TRequest, TResponse, TResponse2\>

Namespace: MassTransit

```csharp
public interface IRequestConfigurator<TInstance, TRequest, TResponse, TResponse2> : IRequestConfigurator<TInstance, TRequest, TResponse>, IRequestConfigurator
```

#### Type Parameters

`TInstance`<br/>

`TRequest`<br/>

`TResponse`<br/>

`TResponse2`<br/>

Implements [IRequestConfigurator\<TInstance, TRequest, TResponse\>](../masstransit/irequestconfigurator-3), [IRequestConfigurator](../masstransit/irequestconfigurator)

## Properties

### **Completed2**

Configure the behavior of the Completed event, the same was Events are configured on
 the state machine.

```csharp
public abstract Action<IEventCorrelationConfigurator<TInstance, TResponse2>> Completed2 { set; }
```

#### Property Value

[Action\<IEventCorrelationConfigurator\<TInstance, TResponse2\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
