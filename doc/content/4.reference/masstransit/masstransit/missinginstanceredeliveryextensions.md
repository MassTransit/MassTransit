---

title: MissingInstanceRedeliveryExtensions

---

# MissingInstanceRedeliveryExtensions

Namespace: MassTransit

```csharp
public static class MissingInstanceRedeliveryExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MissingInstanceRedeliveryExtensions](../masstransit/missinginstanceredeliveryextensions)

## Methods

### **Redeliver\<TInstance, TData\>(IMissingInstanceConfigurator\<TInstance, TData\>, Action\<IMissingInstanceRedeliveryConfigurator\<TInstance, TData\>\>)**

Redeliver uses the message scheduler to deliver the message to the queue at a future
 time. The delivery count is incremented.
 A message scheduler must be configured on the bus for redelivery to be enabled.

```csharp
public static IPipe<ConsumeContext<TData>> Redeliver<TInstance, TData>(IMissingInstanceConfigurator<TInstance, TData> configurator, Action<IMissingInstanceRedeliveryConfigurator<TInstance, TData>> configure)
```

#### Type Parameters

`TInstance`<br/>
The instance type

`TData`<br/>
The event data type

#### Parameters

`configurator` [IMissingInstanceConfigurator\<TInstance, TData\>](../../masstransit-abstractions/masstransit/imissinginstanceconfigurator-2)<br/>
The consume context of the message

`configure` [Action\<IMissingInstanceRedeliveryConfigurator\<TInstance, TData\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
Configure the retry policy for the message redelivery

#### Returns

[IPipe\<ConsumeContext\<TData\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>
