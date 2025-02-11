---

title: IHashGenerator

---

# IHashGenerator

Namespace: MassTransit.Middleware

Generates a hash of the input data for partitioning purposes

```csharp
public interface IHashGenerator
```

## Methods

### **Hash(Byte[])**

```csharp
uint Hash(Byte[] data)
```

#### Parameters

`data` [Byte[]](https://learn.microsoft.com/en-us/dotnet/api/system.byte)<br/>

#### Returns

[UInt32](https://learn.microsoft.com/en-us/dotnet/api/system.uint32)<br/>
