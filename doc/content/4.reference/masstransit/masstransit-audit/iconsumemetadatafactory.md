---

title: IConsumeMetadataFactory

---

# IConsumeMetadataFactory

Namespace: MassTransit.Audit

```csharp
public interface IConsumeMetadataFactory
```

## Methods

### **CreateAuditMetadata\<T\>(ConsumeContext\<T\>)**

```csharp
MessageAuditMetadata CreateAuditMetadata<T>(ConsumeContext<T> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [ConsumeContext\<T\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

#### Returns

[MessageAuditMetadata](../masstransit-audit/messageauditmetadata)<br/>
