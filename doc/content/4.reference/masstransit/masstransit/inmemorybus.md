---

title: InMemoryBus

---

# InMemoryBus

Namespace: MassTransit

```csharp
public static class InMemoryBus
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [InMemoryBus](../masstransit/inmemorybus)

## Methods

### **Create(Action\<IInMemoryBusFactoryConfigurator\>)**

Configure and create an in-memory bus

```csharp
public static IBusControl Create(Action<IInMemoryBusFactoryConfigurator> configure)
```

#### Parameters

`configure` [Action\<IInMemoryBusFactoryConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
The configuration callback to configure the bus

#### Returns

[IBusControl](../../masstransit-abstractions/masstransit/ibuscontrol)<br/>

### **Create(Uri, Action\<IInMemoryBusFactoryConfigurator\>)**

Configure and create an in-memory bus

```csharp
public static IBusControl Create(Uri baseAddress, Action<IInMemoryBusFactoryConfigurator> configure)
```

#### Parameters

`baseAddress` Uri<br/>
Override the default base address

`configure` [Action\<IInMemoryBusFactoryConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
The configuration callback to configure the bus

#### Returns

[IBusControl](../../masstransit-abstractions/masstransit/ibuscontrol)<br/>

### **CreateMessageTopology()**

```csharp
public static IMessageTopologyConfigurator CreateMessageTopology()
```

#### Returns

[IMessageTopologyConfigurator](../../masstransit-abstractions/masstransit-configuration/imessagetopologyconfigurator)<br/>
