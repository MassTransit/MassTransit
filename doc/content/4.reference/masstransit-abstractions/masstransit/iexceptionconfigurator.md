---

title: IExceptionConfigurator

---

# IExceptionConfigurator

Namespace: MassTransit

```csharp
public interface IExceptionConfigurator
```

## Methods

### **Handle(Type[])**

```csharp
void Handle(Type[] exceptionTypes)
```

#### Parameters

`exceptionTypes` [Type[]](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

### **Handle\<T\>()**

```csharp
void Handle<T>()
```

#### Type Parameters

`T`<br/>

### **Handle\<T\>(Func\<T, Boolean\>)**

```csharp
void Handle<T>(Func<T, bool> filter)
```

#### Type Parameters

`T`<br/>

#### Parameters

`filter` [Func\<T, Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

### **Ignore(Type[])**

```csharp
void Ignore(Type[] exceptionTypes)
```

#### Parameters

`exceptionTypes` [Type[]](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

### **Ignore\<T\>()**

```csharp
void Ignore<T>()
```

#### Type Parameters

`T`<br/>

### **Ignore\<T\>(Func\<T, Boolean\>)**

```csharp
void Ignore<T>(Func<T, bool> filter)
```

#### Type Parameters

`T`<br/>

#### Parameters

`filter` [Func\<T, Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
