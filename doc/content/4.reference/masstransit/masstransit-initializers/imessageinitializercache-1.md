---

title: IMessageInitializerCache<TMessage>

---

# IMessageInitializerCache\<TMessage\>

Namespace: MassTransit.Initializers

```csharp
public interface IMessageInitializerCache<TMessage>
```

#### Type Parameters

`TMessage`<br/>

## Methods

### **GetInitializer(Type)**

```csharp
IMessageInitializer<TMessage> GetInitializer(Type objectType)
```

#### Parameters

`objectType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[IMessageInitializer\<TMessage\>](../../masstransit-abstractions/masstransit-initializers/imessageinitializer-1)<br/>
