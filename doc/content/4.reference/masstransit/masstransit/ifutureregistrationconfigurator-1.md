---

title: IFutureRegistrationConfigurator<TFuture>

---

# IFutureRegistrationConfigurator\<TFuture\>

Namespace: MassTransit

```csharp
public interface IFutureRegistrationConfigurator<TFuture> : IFutureRegistrationConfigurator
```

#### Type Parameters

`TFuture`<br/>

Implements [IFutureRegistrationConfigurator](../masstransit/ifutureregistrationconfigurator)

## Methods

### **Endpoint(Action\<IEndpointRegistrationConfigurator\>)**

```csharp
IFutureRegistrationConfigurator<TFuture> Endpoint(Action<IEndpointRegistrationConfigurator> configure)
```

#### Parameters

`configure` [Action\<IEndpointRegistrationConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[IFutureRegistrationConfigurator\<TFuture\>](../masstransit/ifutureregistrationconfigurator-1)<br/>

### **Repository(Action\<ISagaRepositoryRegistrationConfigurator\<FutureState\>\>)**

```csharp
IFutureRegistrationConfigurator<TFuture> Repository(Action<ISagaRepositoryRegistrationConfigurator<FutureState>> configure)
```

#### Parameters

`configure` [Action\<ISagaRepositoryRegistrationConfigurator\<FutureState\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[IFutureRegistrationConfigurator\<TFuture\>](../masstransit/ifutureregistrationconfigurator-1)<br/>
