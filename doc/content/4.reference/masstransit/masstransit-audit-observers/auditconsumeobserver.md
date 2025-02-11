---

title: AuditConsumeObserver

---

# AuditConsumeObserver

Namespace: MassTransit.Audit.Observers

```csharp
public class AuditConsumeObserver : IConsumeObserver
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [AuditConsumeObserver](../masstransit-audit-observers/auditconsumeobserver)<br/>
Implements [IConsumeObserver](../../masstransit-abstractions/masstransit/iconsumeobserver)

## Constructors

### **AuditConsumeObserver(IMessageAuditStore, IConsumeMetadataFactory, CompositeFilter\<ConsumeContext\>)**

```csharp
public AuditConsumeObserver(IMessageAuditStore store, IConsumeMetadataFactory metadataFactory, CompositeFilter<ConsumeContext> filter)
```

#### Parameters

`store` [IMessageAuditStore](../masstransit-audit/imessageauditstore)<br/>

`metadataFactory` [IConsumeMetadataFactory](../masstransit-audit/iconsumemetadatafactory)<br/>

`filter` [CompositeFilter\<ConsumeContext\>](../masstransit-configuration/compositefilter-1)<br/>
