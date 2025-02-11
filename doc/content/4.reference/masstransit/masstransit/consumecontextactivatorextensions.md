---

title: ConsumeContextActivatorExtensions

---

# ConsumeContextActivatorExtensions

Namespace: MassTransit

```csharp
public static class ConsumeContextActivatorExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConsumeContextActivatorExtensions](../masstransit/consumecontextactivatorextensions)

## Methods

### **GetServiceOrCreateInstance\<T\>(ConsumeContext)**

If the  has an  or  payload,
 use that payload to get the service or create an instance of the specified type.

```csharp
public static T GetServiceOrCreateInstance<T>(ConsumeContext context)
```

#### Type Parameters

`T`<br/>
The service type

#### Parameters

`context` [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>

#### Returns

T<br/>

#### Exceptions

T:MassTransit.PayloadNotFoundException<br/>

### **CreateInstance\<T\>(ConsumeContext, Object[])**

If the  has an  or  payload,
 use that payload to create an instance of the specified type.

```csharp
public static T CreateInstance<T>(ConsumeContext context, Object[] arguments)
```

#### Type Parameters

`T`<br/>
The service type

#### Parameters

`context` [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>

`arguments` [Object[]](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

T<br/>

#### Exceptions

T:MassTransit.PayloadNotFoundException<br/>
