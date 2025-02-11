---

title: ICryptoStreamProvider

---

# ICryptoStreamProvider

Namespace: MassTransit.Serialization

Provides a crypto stream for the purpose of encrypting or decrypting

```csharp
public interface ICryptoStreamProvider : IProbeSite
```

Implements [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Methods

### **GetEncryptStream(Stream, String, CryptoStreamMode)**

Returns a stream with the encryption bits in place to ensure proper message encryption

```csharp
Stream GetEncryptStream(Stream stream, string keyId, CryptoStreamMode streamMode)
```

#### Parameters

`stream` [Stream](https://learn.microsoft.com/en-us/dotnet/api/system.io.stream)<br/>
The original stream to which the encrypted message content is written

`keyId` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The encryption key identifier

`streamMode` CryptoStreamMode<br/>

#### Returns

[Stream](https://learn.microsoft.com/en-us/dotnet/api/system.io.stream)<br/>
A stream for serializing the message which will be encrypted

### **GetDecryptStream(Stream, String, CryptoStreamMode)**

Returns a stream for decrypting the message

```csharp
Stream GetDecryptStream(Stream stream, string keyId, CryptoStreamMode streamMode)
```

#### Parameters

`stream` [Stream](https://learn.microsoft.com/en-us/dotnet/api/system.io.stream)<br/>
The input stream of the encrypted message

`keyId` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The encryption key identifier

`streamMode` CryptoStreamMode<br/>

#### Returns

[Stream](https://learn.microsoft.com/en-us/dotnet/api/system.io.stream)<br/>
A stream for deserializing the encrypted message
