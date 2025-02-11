---

title: ConstantSecureKeyProvider

---

# ConstantSecureKeyProvider

Namespace: MassTransit.Serialization

```csharp
public class ConstantSecureKeyProvider : ISecureKeyProvider, IProbeSite
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConstantSecureKeyProvider](../masstransit-serialization/constantsecurekeyprovider)<br/>
Implements [ISecureKeyProvider](../masstransit-serialization/isecurekeyprovider), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **ConstantSecureKeyProvider(Byte[])**

```csharp
public ConstantSecureKeyProvider(Byte[] key)
```

#### Parameters

`key` [Byte[]](https://learn.microsoft.com/en-us/dotnet/api/system.byte)<br/>

## Methods

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>

### **GetKey(Headers)**

```csharp
public Byte[] GetKey(Headers headers)
```

#### Parameters

`headers` [Headers](../../masstransit-abstractions/masstransit/headers)<br/>

#### Returns

[Byte[]](https://learn.microsoft.com/en-us/dotnet/api/system.byte)<br/>
