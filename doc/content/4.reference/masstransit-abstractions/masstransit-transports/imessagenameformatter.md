---

title: IMessageNameFormatter

---

# IMessageNameFormatter

Namespace: MassTransit.Transports

Used to format a message type into a MessageName, which can be used as a valid
 queue name on the transport

```csharp
public interface IMessageNameFormatter
```

## Methods

### **GetMessageName(Type)**

```csharp
string GetMessageName(Type type)
```

#### Parameters

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
