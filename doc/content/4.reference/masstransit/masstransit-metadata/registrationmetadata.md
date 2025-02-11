---

title: RegistrationMetadata

---

# RegistrationMetadata

Namespace: MassTransit.Metadata

```csharp
public static class RegistrationMetadata
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RegistrationMetadata](../masstransit-metadata/registrationmetadata)

## Methods

### **IsConsumerOrDefinition(Type)**

Returns true if the type is a consumer, or a consumer definition

```csharp
public static bool IsConsumerOrDefinition(Type type)
```

#### Parameters

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **IsConsumer(Type)**

Returns true if the type is a consumer, or a consumer definition

```csharp
public static bool IsConsumer(Type type)
```

#### Parameters

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **IsSagaOrDefinition(Type)**

Returns true if the type is a saga, or a saga definition

```csharp
public static bool IsSagaOrDefinition(Type type)
```

#### Parameters

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **IsSaga(Type)**

Returns true if the type is a saga

```csharp
public static bool IsSaga(Type type)
```

#### Parameters

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **IsSagaStateMachineOrDefinition(Type)**

Returns true if the type is a state machine or saga definition

```csharp
public static bool IsSagaStateMachineOrDefinition(Type type)
```

#### Parameters

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **IsActivityOrDefinition(Type)**

Returns true if the type is an activity

```csharp
public static bool IsActivityOrDefinition(Type type)
```

#### Parameters

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **IsFutureOrDefinition(Type)**

Returns true if the type is a future or future definition

```csharp
public static bool IsFutureOrDefinition(Type type)
```

#### Parameters

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
