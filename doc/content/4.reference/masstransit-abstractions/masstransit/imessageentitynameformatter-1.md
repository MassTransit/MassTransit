---

title: IMessageEntityNameFormatter<TMessage>

---

# IMessageEntityNameFormatter\<TMessage\>

Namespace: MassTransit

Used to build entity names for the publish topology

```csharp
public interface IMessageEntityNameFormatter<TMessage>
```

#### Type Parameters

`TMessage`<br/>

## Methods

### **FormatEntityName()**

Formats the entity name for the given message

```csharp
string FormatEntityName()
```

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
