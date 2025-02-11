---

title: IEntityNameFormatter

---

# IEntityNameFormatter

Namespace: MassTransit

Used to build entity names for the publish topology

```csharp
public interface IEntityNameFormatter
```

## Methods

### **FormatEntityName\<T\>()**

Formats the entity name for the given message type

```csharp
string FormatEntityName<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
