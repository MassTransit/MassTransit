---

title: ICachePolicy<TValue, TCacheValue>

---

# ICachePolicy\<TValue, TCacheValue\>

Namespace: MassTransit.Internals.Caching

```csharp
public interface ICachePolicy<TValue, TCacheValue>
```

#### Type Parameters

`TValue`<br/>

`TCacheValue`<br/>

## Methods

### **CreateValue(Action)**

```csharp
TCacheValue CreateValue(Action remove)
```

#### Parameters

`remove` [Action](https://learn.microsoft.com/en-us/dotnet/api/system.action)<br/>

#### Returns

TCacheValue<br/>

### **IsValid(TCacheValue)**

```csharp
bool IsValid(TCacheValue value)
```

#### Parameters

`value` TCacheValue<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **CheckValue(TCacheValue)**

```csharp
int CheckValue(TCacheValue value)
```

#### Parameters

`value` TCacheValue<br/>

#### Returns

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
