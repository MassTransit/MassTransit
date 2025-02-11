---

title: AuditConfigurationExtensions

---

# AuditConfigurationExtensions

Namespace: MassTransit

```csharp
public static class AuditConfigurationExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [AuditConfigurationExtensions](../masstransit/auditconfigurationextensions)

## Methods

### **ConnectSendAuditObservers\<T\>(T, IMessageAuditStore, Action\<IMessageFilterConfigurator\>, ISendMetadataFactory)**

Adds observers that will audit all published and sent messages, sending them to the message audit store after they are sent/published.

```csharp
public static ConnectHandle ConnectSendAuditObservers<T>(T connector, IMessageAuditStore store, Action<IMessageFilterConfigurator> configureFilter, ISendMetadataFactory metadataFactory)
```

#### Type Parameters

`T`<br/>

#### Parameters

`connector` T<br/>
The bus

`store` [IMessageAuditStore](../masstransit-audit/imessageauditstore)<br/>
Audit store

`configureFilter` [Action\<IMessageFilterConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
Filter configuration delegate

`metadataFactory` [ISendMetadataFactory](../masstransit-audit/isendmetadatafactory)<br/>
Message metadata factory. If omitted, the default one will be used.

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **ConnectConsumeAuditObserver(IConsumeObserverConnector, IMessageAuditStore, Action\<IMessageFilterConfigurator\>, IConsumeMetadataFactory)**

Add an observer that will audit consumed messages, sending them to the message audit store prior to consumption by the consumer

```csharp
public static ConnectHandle ConnectConsumeAuditObserver(IConsumeObserverConnector connector, IMessageAuditStore store, Action<IMessageFilterConfigurator> configureFilter, IConsumeMetadataFactory metadataFactory)
```

#### Parameters

`connector` [IConsumeObserverConnector](../../masstransit-abstractions/masstransit/iconsumeobserverconnector)<br/>
The bus or endpoint

`store` [IMessageAuditStore](../masstransit-audit/imessageauditstore)<br/>
The audit store

`configureFilter` [Action\<IMessageFilterConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
Filter configuration delegate

`metadataFactory` [IConsumeMetadataFactory](../masstransit-audit/iconsumemetadatafactory)<br/>
Message metadata factory. If omitted, the default one will be used.

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>
