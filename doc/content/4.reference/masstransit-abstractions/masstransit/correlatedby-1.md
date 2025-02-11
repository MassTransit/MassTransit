---

title: CorrelatedBy<TKey>

---

# CorrelatedBy\<TKey\>

Namespace: MassTransit

Used to identify a message as correlated so that the CorrelationId can be returned

```csharp
public interface CorrelatedBy<TKey>
```

#### Type Parameters

`TKey`<br/>
The type of the CorrelationId used

## Properties

### **CorrelationId**

Returns the CorrelationId for the message

```csharp
public abstract TKey CorrelationId { get; }
```

#### Property Value

TKey<br/>
