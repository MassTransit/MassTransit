---

title: MessageTypeFilterConfigurator

---

# MessageTypeFilterConfigurator

Namespace: MassTransit.Configuration

```csharp
public class MessageTypeFilterConfigurator : IMessageTypeFilterConfigurator
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessageTypeFilterConfigurator](../masstransit-configuration/messagetypefilterconfigurator)<br/>
Implements [IMessageTypeFilterConfigurator](../../masstransit-abstractions/masstransit/imessagetypefilterconfigurator)

## Properties

### **Filter**

```csharp
public CompositeFilter<Type> Filter { get; }
```

#### Property Value

[CompositeFilter\<Type\>](../masstransit-configuration/compositefilter-1)<br/>

## Constructors

### **MessageTypeFilterConfigurator()**

```csharp
public MessageTypeFilterConfigurator()
```

## Methods

### **Include(Type[])**

```csharp
public void Include(Type[] messageTypes)
```

#### Parameters

`messageTypes` [Type[]](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

### **Include(Func\<Type, Boolean\>)**

```csharp
public void Include(Func<Type, bool> filter)
```

#### Parameters

`filter` [Func\<Type, Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

### **Include\<T\>()**

```csharp
public void Include<T>()
```

#### Type Parameters

`T`<br/>

### **Exclude(Type[])**

```csharp
public void Exclude(Type[] messageTypes)
```

#### Parameters

`messageTypes` [Type[]](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

### **Exclude(Func\<Type, Boolean\>)**

```csharp
public void Exclude(Func<Type, bool> filter)
```

#### Parameters

`filter` [Func\<Type, Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

### **Exclude\<T\>()**

```csharp
public void Exclude<T>()
```

#### Type Parameters

`T`<br/>
