---

title: ISendMetadataFactory

---

# ISendMetadataFactory

Namespace: MassTransit.Audit

```csharp
public interface ISendMetadataFactory
```

## Methods

### **CreateAuditMetadata\<T\>(SendContext\<T\>)**

```csharp
MessageAuditMetadata CreateAuditMetadata<T>(SendContext<T> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [SendContext\<T\>](../../masstransit-abstractions/masstransit/sendcontext-1)<br/>

#### Returns

[MessageAuditMetadata](../masstransit-audit/messageauditmetadata)<br/>

### **CreateAuditMetadata\<T\>(PublishContext\<T\>)**

```csharp
MessageAuditMetadata CreateAuditMetadata<T>(PublishContext<T> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [PublishContext\<T\>](../../masstransit-abstractions/masstransit/publishcontext-1)<br/>

#### Returns

[MessageAuditMetadata](../masstransit-audit/messageauditmetadata)<br/>
