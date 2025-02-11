---

title: MediatorConfigurationExtensions

---

# MediatorConfigurationExtensions

Namespace: MassTransit

```csharp
public static class MediatorConfigurationExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MediatorConfigurationExtensions](../masstransit/mediatorconfigurationextensions)

## Methods

### **CreateMediator(IBusFactorySelector, Action\<IMediatorConfigurator\>)**

Create a mediator, which sends messages to consumers, handlers, and sagas. Messages are dispatched to the consumers asynchronously.
 Consumers are not directly coupled to the sender. Can be used entirely in-memory without a broker.

```csharp
public static IMediator CreateMediator(IBusFactorySelector selector, Action<IMediatorConfigurator> configure)
```

#### Parameters

`selector` [IBusFactorySelector](../masstransit/ibusfactoryselector)<br/>

`configure` [Action\<IMediatorConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[IMediator](../../masstransit-abstractions/masstransit-mediator/imediator)<br/>

#### Exceptions

[ArgumentNullException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentnullexception)<br/>

### **CreateMediator(IBusFactorySelector, Uri, Action\<IMediatorConfigurator\>)**

Create a mediator, which sends messages to consumers, handlers, and sagas. Messages are dispatched to the consumers asynchronously.
 Consumers are not directly coupled to the sender. Can be used entirely in-memory without a broker.

```csharp
public static IMediator CreateMediator(IBusFactorySelector selector, Uri baseAddress, Action<IMediatorConfigurator> configure)
```

#### Parameters

`selector` [IBusFactorySelector](../masstransit/ibusfactoryselector)<br/>

`baseAddress` Uri<br/>

`configure` [Action\<IMediatorConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[IMediator](../../masstransit-abstractions/masstransit-mediator/imediator)<br/>

#### Exceptions

[ArgumentNullException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentnullexception)<br/>
