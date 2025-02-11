---

title: IMessageFilterConfigurator

---

# IMessageFilterConfigurator

Namespace: MassTransit

Configures a message filter, for including and excluding message types

```csharp
public interface IMessageFilterConfigurator : IMessageTypeFilterConfigurator
```

Implements [IMessageTypeFilterConfigurator](../masstransit/imessagetypefilterconfigurator)

## Methods

### **Include\<T\>(Func\<T, Boolean\>)**

Include the message if it is the specified message type and matches the specified filter expression

```csharp
void Include<T>(Func<T, bool> filter)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`filter` [Func\<T, Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
The filter expression

### **Exclude\<T\>(Func\<T, Boolean\>)**

Exclude the message if it is the specified message type and matches the specified filter expression

```csharp
void Exclude<T>(Func<T, bool> filter)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`filter` [Func\<T, Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
The filter expression
