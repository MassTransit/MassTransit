---

title: ExceptionSpecification

---

# ExceptionSpecification

Namespace: MassTransit.Configuration

```csharp
public abstract class ExceptionSpecification : IExceptionConfigurator
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ExceptionSpecification](../masstransit-configuration/exceptionspecification)<br/>
Implements [IExceptionConfigurator](../masstransit/iexceptionconfigurator)

## Methods

### **Handle(Type[])**

```csharp
public void Handle(Type[] exceptionTypes)
```

#### Parameters

`exceptionTypes` [Type[]](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

### **Handle\<T\>()**

```csharp
public void Handle<T>()
```

#### Type Parameters

`T`<br/>

### **Handle\<T\>(Func\<T, Boolean\>)**

```csharp
public void Handle<T>(Func<T, bool> filter)
```

#### Type Parameters

`T`<br/>

#### Parameters

`filter` [Func\<T, Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

### **Ignore(Type[])**

```csharp
public void Ignore(Type[] exceptionTypes)
```

#### Parameters

`exceptionTypes` [Type[]](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

### **Ignore\<T\>()**

```csharp
public void Ignore<T>()
```

#### Type Parameters

`T`<br/>

### **Ignore\<T\>(Func\<T, Boolean\>)**

```csharp
public void Ignore<T>(Func<T, bool> filter)
```

#### Type Parameters

`T`<br/>

#### Parameters

`filter` [Func\<T, Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
