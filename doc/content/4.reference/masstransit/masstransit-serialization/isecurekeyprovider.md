---

title: ISecureKeyProvider

---

# ISecureKeyProvider

Namespace: MassTransit.Serialization

```csharp
public interface ISecureKeyProvider : IProbeSite
```

Implements [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Methods

### **GetKey(Headers)**

```csharp
Byte[] GetKey(Headers headers)
```

#### Parameters

`headers` [Headers](../../masstransit-abstractions/masstransit/headers)<br/>

#### Returns

[Byte[]](https://learn.microsoft.com/en-us/dotnet/api/system.byte)<br/>
