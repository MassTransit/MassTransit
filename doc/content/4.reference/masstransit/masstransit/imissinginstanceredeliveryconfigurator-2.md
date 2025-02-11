---

title: IMissingInstanceRedeliveryConfigurator<TInstance, TData>

---

# IMissingInstanceRedeliveryConfigurator\<TInstance, TData\>

Namespace: MassTransit

```csharp
public interface IMissingInstanceRedeliveryConfigurator<TInstance, TData> : IMissingInstanceRedeliveryConfigurator, IRedeliveryConfigurator, IRetryConfigurator, IExceptionConfigurator, IRetryObserverConnector
```

#### Type Parameters

`TInstance`<br/>

`TData`<br/>

Implements [IMissingInstanceRedeliveryConfigurator](../masstransit/imissinginstanceredeliveryconfigurator), [IRedeliveryConfigurator](../masstransit/iredeliveryconfigurator), [IRetryConfigurator](../masstransit/iretryconfigurator), [IExceptionConfigurator](../../masstransit-abstractions/masstransit/iexceptionconfigurator), [IRetryObserverConnector](../../masstransit-abstractions/masstransit/iretryobserverconnector)

## Methods

### **OnRedeliveryLimitReached(Func\<IMissingInstanceConfigurator\<TInstance, TData\>, IPipe\<ConsumeContext\<TData\>\>\>)**

```csharp
void OnRedeliveryLimitReached(Func<IMissingInstanceConfigurator<TInstance, TData>, IPipe<ConsumeContext<TData>>> configure)
```

#### Parameters

`configure` [Func\<IMissingInstanceConfigurator\<TInstance, TData\>, IPipe\<ConsumeContext\<TData\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
