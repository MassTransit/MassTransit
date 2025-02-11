---

title: ISymmetricKeyProvider

---

# ISymmetricKeyProvider

Namespace: MassTransit.Serialization

Returns the symmetric key used to encrypt or decrypt messages

```csharp
public interface ISymmetricKeyProvider
```

## Methods

### **TryGetKey(String, SymmetricKey)**

Return the specified key, if found. When using Symmetric key encryption, the default key is used
 unless the transport header contains a specific key identifier for the message.

```csharp
bool TryGetKey(string id, out SymmetricKey key)
```

#### Parameters

`id` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The key id

`key` [SymmetricKey](../masstransit-serialization/symmetrickey)<br/>
The symmetric key

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
