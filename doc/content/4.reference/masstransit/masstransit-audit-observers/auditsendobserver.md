---

title: AuditSendObserver

---

# AuditSendObserver

Namespace: MassTransit.Audit.Observers

```csharp
public class AuditSendObserver : ISendObserver
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [AuditSendObserver](../masstransit-audit-observers/auditsendobserver)<br/>
Implements [ISendObserver](../../masstransit-abstractions/masstransit/isendobserver)

## Constructors

### **AuditSendObserver(IMessageAuditStore, ISendMetadataFactory, CompositeFilter\<SendContext\>)**

```csharp
public AuditSendObserver(IMessageAuditStore store, ISendMetadataFactory metadataFactory, CompositeFilter<SendContext> filter)
```

#### Parameters

`store` [IMessageAuditStore](../masstransit-audit/imessageauditstore)<br/>

`metadataFactory` [ISendMetadataFactory](../masstransit-audit/isendmetadatafactory)<br/>

`filter` [CompositeFilter\<SendContext\>](../masstransit-configuration/compositefilter-1)<br/>
