---

title: IRegistrationFilterConfigurator

---

# IRegistrationFilterConfigurator

Namespace: MassTransit.DependencyInjection.Registration

Specify the consumer, saga, and activity types to include/exclude

```csharp
public interface IRegistrationFilterConfigurator
```

## Methods

### **Include(Type[])**

Include the specified types

```csharp
void Include(Type[] types)
```

#### Parameters

`types` [Type[]](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

### **Include\<T\>()**

Include the specified type

```csharp
void Include<T>()
```

#### Type Parameters

`T`<br/>

### **Exclude(Type[])**

Exclude the specified types

```csharp
void Exclude(Type[] types)
```

#### Parameters

`types` [Type[]](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

### **Exclude\<T\>()**

Exclude the specified type

```csharp
void Exclude<T>()
```

#### Type Parameters

`T`<br/>
