---

title: MessageDataRepositorySelectorExtensions

---

# MessageDataRepositorySelectorExtensions

Namespace: MassTransit.MessageData.Configuration

```csharp
public static class MessageDataRepositorySelectorExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessageDataRepositorySelectorExtensions](../masstransit-messagedata-configuration/messagedatarepositoryselectorextensions)

## Methods

### **InMemory(IMessageDataRepositorySelector)**

```csharp
public static IMessageDataRepository InMemory(IMessageDataRepositorySelector selector)
```

#### Parameters

`selector` [IMessageDataRepositorySelector](../masstransit-configuration/imessagedatarepositoryselector)<br/>

#### Returns

[IMessageDataRepository](../../masstransit-abstractions/masstransit/imessagedatarepository)<br/>

### **FileSystem(IMessageDataRepositorySelector, String)**

```csharp
public static IMessageDataRepository FileSystem(IMessageDataRepositorySelector selector, string path)
```

#### Parameters

`selector` [IMessageDataRepositorySelector](../masstransit-configuration/imessagedatarepositoryselector)<br/>

`path` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[IMessageDataRepository](../../masstransit-abstractions/masstransit/imessagedatarepository)<br/>

### **Encrypted(IMessageDataRepositorySelector, ICryptoStreamProvider, Func\<IMessageDataRepositorySelector, IMessageDataRepository\>)**

```csharp
public static IMessageDataRepository Encrypted(IMessageDataRepositorySelector selector, ICryptoStreamProvider streamProvider, Func<IMessageDataRepositorySelector, IMessageDataRepository> innerSelector)
```

#### Parameters

`selector` [IMessageDataRepositorySelector](../masstransit-configuration/imessagedatarepositoryselector)<br/>

`streamProvider` [ICryptoStreamProvider](../masstransit-serialization/icryptostreamprovider)<br/>

`innerSelector` [Func\<IMessageDataRepositorySelector, IMessageDataRepository\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[IMessageDataRepository](../../masstransit-abstractions/masstransit/imessagedatarepository)<br/>
