---

title: AesCryptoStreamProviderV2

---

# AesCryptoStreamProviderV2

Namespace: MassTransit.Serialization

```csharp
public class AesCryptoStreamProviderV2 : ICryptoStreamProviderV2, IProbeSite
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [AesCryptoStreamProviderV2](../masstransit-serialization/aescryptostreamproviderv2)<br/>
Implements [ICryptoStreamProviderV2](../masstransit-serialization/icryptostreamproviderv2), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **AesCryptoStreamProviderV2(ISecureKeyProvider, PaddingMode)**

```csharp
public AesCryptoStreamProviderV2(ISecureKeyProvider secureKeyProvider, PaddingMode paddingMode)
```

#### Parameters

`secureKeyProvider` [ISecureKeyProvider](../masstransit-serialization/isecurekeyprovider)<br/>

`paddingMode` PaddingMode<br/>

## Methods

### **GetDecryptStream(Stream, Headers)**

```csharp
public Stream GetDecryptStream(Stream stream, Headers headers)
```

#### Parameters

`stream` [Stream](https://learn.microsoft.com/en-us/dotnet/api/system.io.stream)<br/>

`headers` [Headers](../../masstransit-abstractions/masstransit/headers)<br/>

#### Returns

[Stream](https://learn.microsoft.com/en-us/dotnet/api/system.io.stream)<br/>

### **GetEncryptStream(Stream, Headers)**

```csharp
public Stream GetEncryptStream(Stream stream, Headers headers)
```

#### Parameters

`stream` [Stream](https://learn.microsoft.com/en-us/dotnet/api/system.io.stream)<br/>

`headers` [Headers](../../masstransit-abstractions/masstransit/headers)<br/>

#### Returns

[Stream](https://learn.microsoft.com/en-us/dotnet/api/system.io.stream)<br/>

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>
