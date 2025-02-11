---

title: ICryptoStreamProviderV2

---

# ICryptoStreamProviderV2

Namespace: MassTransit.Serialization

```csharp
public interface ICryptoStreamProviderV2 : IProbeSite
```

Implements [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Methods

### **GetDecryptStream(Stream, Headers)**

```csharp
Stream GetDecryptStream(Stream stream, Headers headers)
```

#### Parameters

`stream` [Stream](https://learn.microsoft.com/en-us/dotnet/api/system.io.stream)<br/>

`headers` [Headers](../../masstransit-abstractions/masstransit/headers)<br/>

#### Returns

[Stream](https://learn.microsoft.com/en-us/dotnet/api/system.io.stream)<br/>

### **GetEncryptStream(Stream, Headers)**

```csharp
Stream GetEncryptStream(Stream stream, Headers headers)
```

#### Parameters

`stream` [Stream](https://learn.microsoft.com/en-us/dotnet/api/system.io.stream)<br/>

`headers` [Headers](../../masstransit-abstractions/masstransit/headers)<br/>

#### Returns

[Stream](https://learn.microsoft.com/en-us/dotnet/api/system.io.stream)<br/>
