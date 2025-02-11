---

title: ISqlSendTopology

---

# ISqlSendTopology

Namespace: MassTransit

```csharp
public interface ISqlSendTopology : ISendTopology, ISendTopologyConfigurationObserverConnector
```

Implements [ISendTopology](../../masstransit-abstractions/masstransit/isendtopology), [ISendTopologyConfigurationObserverConnector](../../masstransit-abstractions/masstransit-configuration/isendtopologyconfigurationobserverconnector)

## Methods

### **GetMessageTopology\<T\>()**

```csharp
ISqlMessageSendTopologyConfigurator<T> GetMessageTopology<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[ISqlMessageSendTopologyConfigurator\<T\>](../masstransit/isqlmessagesendtopologyconfigurator-1)<br/>

### **GetSendSettings(SqlEndpointAddress)**

Return the send settings for the specified

```csharp
SendSettings GetSendSettings(SqlEndpointAddress address)
```

#### Parameters

`address` [SqlEndpointAddress](../masstransit/sqlendpointaddress)<br/>

#### Returns

[SendSettings](../masstransit-sqltransport/sendsettings)<br/>

### **GetErrorSettings(ReceiveSettings)**

Return the error settings for the queue

```csharp
SendSettings GetErrorSettings(ReceiveSettings settings)
```

#### Parameters

`settings` [ReceiveSettings](../masstransit-sqltransport/receivesettings)<br/>

#### Returns

[SendSettings](../masstransit-sqltransport/sendsettings)<br/>

### **GetDeadLetterSettings(ReceiveSettings)**

Return the dead letter settings for the queue

```csharp
SendSettings GetDeadLetterSettings(ReceiveSettings settings)
```

#### Parameters

`settings` [ReceiveSettings](../masstransit-sqltransport/receivesettings)<br/>

#### Returns

[SendSettings](../masstransit-sqltransport/sendsettings)<br/>
