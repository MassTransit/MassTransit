---

title: AesCryptoStreamProvider

---

# AesCryptoStreamProvider

Namespace: MassTransit.Serialization

```csharp
public class AesCryptoStreamProvider : ICryptoStreamProvider, IProbeSite
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [AesCryptoStreamProvider](../masstransit-serialization/aescryptostreamprovider)<br/>
Implements [ICryptoStreamProvider](../masstransit-serialization/icryptostreamprovider), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **AesCryptoStreamProvider(ISymmetricKeyProvider, String, PaddingMode)**

```csharp
public AesCryptoStreamProvider(ISymmetricKeyProvider keyProvider, string defaultKeyId, PaddingMode paddingMode)
```

#### Parameters

`keyProvider` [ISymmetricKeyProvider](../masstransit-serialization/isymmetrickeyprovider)<br/>

`defaultKeyId` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`paddingMode` PaddingMode<br/>

## Methods

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>

### **CreateEncryptor(Byte[], Byte[])**

```csharp
public ICryptoTransform CreateEncryptor(Byte[] key, Byte[] iv)
```

#### Parameters

`key` [Byte[]](https://learn.microsoft.com/en-us/dotnet/api/system.byte)<br/>

`iv` [Byte[]](https://learn.microsoft.com/en-us/dotnet/api/system.byte)<br/>

#### Returns

ICryptoTransform<br/>
