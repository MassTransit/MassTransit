---

title: AuditPublishObserver

---

# AuditPublishObserver

Namespace: MassTransit.Audit.Observers

```csharp
public class AuditPublishObserver : IPublishObserver
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [AuditPublishObserver](../masstransit-audit-observers/auditpublishobserver)<br/>
Implements [IPublishObserver](../../masstransit-abstractions/masstransit/ipublishobserver)

## Constructors

### **AuditPublishObserver(IMessageAuditStore, ISendMetadataFactory, CompositeFilter\<SendContext\>)**

```csharp
public AuditPublishObserver(IMessageAuditStore store, ISendMetadataFactory metadataFactory, CompositeFilter<SendContext> filter)
```

#### Parameters

`store` [IMessageAuditStore](../masstransit-audit/imessageauditstore)<br/>

`metadataFactory` [ISendMetadataFactory](../masstransit-audit/isendmetadatafactory)<br/>

`filter` [CompositeFilter\<SendContext\>](../masstransit-configuration/compositefilter-1)<br/>
