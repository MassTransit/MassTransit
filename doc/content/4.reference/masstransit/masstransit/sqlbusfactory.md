---

title: SqlBusFactory

---

# SqlBusFactory

Namespace: MassTransit

```csharp
public static class SqlBusFactory
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SqlBusFactory](../masstransit/sqlbusfactory)

## Methods

### **Create(Action\<ISqlBusFactoryConfigurator\>)**

Create a bus using the database transport

```csharp
public static IBusControl Create(Action<ISqlBusFactoryConfigurator> configure)
```

#### Parameters

`configure` [Action\<ISqlBusFactoryConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
The configuration callback to configure the bus

#### Returns

[IBusControl](../../masstransit-abstractions/masstransit/ibuscontrol)<br/>

### **CreateMessageTopology()**

```csharp
public static IMessageTopologyConfigurator CreateMessageTopology()
```

#### Returns

[IMessageTopologyConfigurator](../../masstransit-abstractions/masstransit-configuration/imessagetopologyconfigurator)<br/>
