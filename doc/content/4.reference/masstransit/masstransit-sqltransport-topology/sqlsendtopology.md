---

title: SqlSendTopology

---

# SqlSendTopology

Namespace: MassTransit.SqlTransport.Topology

```csharp
public class SqlSendTopology : SendTopology, ISendTopologyConfigurator, ISendTopology, ISendTopologyConfigurationObserverConnector, ISpecification, ISendTopologyConfigurationObserver, ISqlSendTopologyConfigurator, ISqlSendTopology
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SendTopology](../../masstransit-abstractions/masstransit-topology/sendtopology) → [SqlSendTopology](../masstransit-sqltransport-topology/sqlsendtopology)<br/>
Implements [ISendTopologyConfigurator](../../masstransit-abstractions/masstransit/isendtopologyconfigurator), [ISendTopology](../../masstransit-abstractions/masstransit/isendtopology), [ISendTopologyConfigurationObserverConnector](../../masstransit-abstractions/masstransit-configuration/isendtopologyconfigurationobserverconnector), [ISpecification](../../masstransit-abstractions/masstransit/ispecification), [ISendTopologyConfigurationObserver](../../masstransit-abstractions/masstransit-configuration/isendtopologyconfigurationobserver), [ISqlSendTopologyConfigurator](../masstransit/isqlsendtopologyconfigurator), [ISqlSendTopology](../masstransit/isqlsendtopology)

## Properties

### **ConfigureErrorSettings**

```csharp
public Action<ISqlQueueConfigurator> ConfigureErrorSettings { get; set; }
```

#### Property Value

[Action\<ISqlQueueConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **ConfigureDeadLetterSettings**

```csharp
public Action<ISqlQueueConfigurator> ConfigureDeadLetterSettings { get; set; }
```

#### Property Value

[Action\<ISqlQueueConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **DeadLetterQueueNameFormatter**

```csharp
public IDeadLetterQueueNameFormatter DeadLetterQueueNameFormatter { get; set; }
```

#### Property Value

[IDeadLetterQueueNameFormatter](../../masstransit-abstractions/masstransit/ideadletterqueuenameformatter)<br/>

### **ErrorQueueNameFormatter**

```csharp
public IErrorQueueNameFormatter ErrorQueueNameFormatter { get; set; }
```

#### Property Value

[IErrorQueueNameFormatter](../../masstransit-abstractions/masstransit/ierrorqueuenameformatter)<br/>

## Constructors

### **SqlSendTopology()**

```csharp
public SqlSendTopology()
```

## Methods

### **GetMessageTopology\<T\>()**

```csharp
public ISqlMessageSendTopologyConfigurator<T> GetMessageTopology<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[ISqlMessageSendTopologyConfigurator\<T\>](../masstransit/isqlmessagesendtopologyconfigurator-1)<br/>

### **GetSendSettings(SqlEndpointAddress)**

```csharp
public SendSettings GetSendSettings(SqlEndpointAddress address)
```

#### Parameters

`address` [SqlEndpointAddress](../masstransit/sqlendpointaddress)<br/>

#### Returns

[SendSettings](../masstransit-sqltransport/sendsettings)<br/>

### **GetErrorSettings(ReceiveSettings)**

```csharp
public SendSettings GetErrorSettings(ReceiveSettings settings)
```

#### Parameters

`settings` [ReceiveSettings](../masstransit-sqltransport/receivesettings)<br/>

#### Returns

[SendSettings](../masstransit-sqltransport/sendsettings)<br/>

### **GetDeadLetterSettings(ReceiveSettings)**

```csharp
public SendSettings GetDeadLetterSettings(ReceiveSettings settings)
```

#### Parameters

`settings` [ReceiveSettings](../masstransit-sqltransport/receivesettings)<br/>

#### Returns

[SendSettings](../masstransit-sqltransport/sendsettings)<br/>

### **CreateMessageTopology\<T\>(Type)**

```csharp
protected IMessageSendTopologyConfigurator CreateMessageTopology<T>(Type type)
```

#### Type Parameters

`T`<br/>

#### Parameters

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[IMessageSendTopologyConfigurator](../../masstransit-abstractions/masstransit/imessagesendtopologyconfigurator)<br/>
