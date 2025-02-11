---

title: ConsumeMessageFilterConfigurator

---

# ConsumeMessageFilterConfigurator

Namespace: MassTransit.Configuration

```csharp
public class ConsumeMessageFilterConfigurator : IMessageFilterConfigurator, IMessageTypeFilterConfigurator
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConsumeMessageFilterConfigurator](../masstransit-configuration/consumemessagefilterconfigurator)<br/>
Implements [IMessageFilterConfigurator](../../masstransit-abstractions/masstransit/imessagefilterconfigurator), [IMessageTypeFilterConfigurator](../../masstransit-abstractions/masstransit/imessagetypefilterconfigurator)

## Properties

### **Filter**

```csharp
public CompositeFilter<ConsumeContext> Filter { get; }
```

#### Property Value

[CompositeFilter\<ConsumeContext\>](../masstransit-configuration/compositefilter-1)<br/>

## Constructors

### **ConsumeMessageFilterConfigurator()**

```csharp
public ConsumeMessageFilterConfigurator()
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

### **Include\<T\>(Func\<T, Boolean\>)**

```csharp
public void Include<T>(Func<T, bool> filter)
```

#### Type Parameters

`T`<br/>

#### Parameters

`filter` [Func\<T, Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

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

### **Exclude\<T\>(Func\<T, Boolean\>)**

```csharp
public void Exclude<T>(Func<T, bool> filter)
```

#### Type Parameters

`T`<br/>

#### Parameters

`filter` [Func\<T, Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
