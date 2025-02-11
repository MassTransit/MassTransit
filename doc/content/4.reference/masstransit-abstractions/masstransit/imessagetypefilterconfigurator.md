---

title: IMessageTypeFilterConfigurator

---

# IMessageTypeFilterConfigurator

Namespace: MassTransit

Configures a message filter, for including and excluding message types

```csharp
public interface IMessageTypeFilterConfigurator
```

## Methods

### **Include(Type[])**

Include the message if it is any of the specified message types

```csharp
void Include(Type[] messageTypes)
```

#### Parameters

`messageTypes` [Type[]](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

### **Include(Func\<Type, Boolean\>)**

Include the type matches the specified filter expression

```csharp
void Include(Func<Type, bool> filter)
```

#### Parameters

`filter` [Func\<Type, Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
The filter expression

### **Include\<T\>()**

Include the message if it is the specified message type

```csharp
void Include<T>()
```

#### Type Parameters

`T`<br/>
The message type

### **Exclude(Type[])**

Exclude the message if it is any of the specified message types

```csharp
void Exclude(Type[] messageTypes)
```

#### Parameters

`messageTypes` [Type[]](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

### **Exclude(Func\<Type, Boolean\>)**

Exclude the type matches the specified filter expression

```csharp
void Exclude(Func<Type, bool> filter)
```

#### Parameters

`filter` [Func\<Type, Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
The filter expression

### **Exclude\<T\>()**

Exclude the message if it is the specified message type

```csharp
void Exclude<T>()
```

#### Type Parameters

`T`<br/>
The message type
