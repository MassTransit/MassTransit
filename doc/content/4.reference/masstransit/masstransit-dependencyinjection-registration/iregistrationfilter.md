---

title: IRegistrationFilter

---

# IRegistrationFilter

Namespace: MassTransit.DependencyInjection.Registration

```csharp
public interface IRegistrationFilter
```

## Methods

### **Matches(IConsumerRegistration)**

```csharp
bool Matches(IConsumerRegistration registration)
```

#### Parameters

`registration` [IConsumerRegistration](../masstransit-configuration/iconsumerregistration)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Matches(ISagaRegistration)**

```csharp
bool Matches(ISagaRegistration registration)
```

#### Parameters

`registration` [ISagaRegistration](../masstransit-configuration/isagaregistration)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Matches(IExecuteActivityRegistration)**

```csharp
bool Matches(IExecuteActivityRegistration registration)
```

#### Parameters

`registration` [IExecuteActivityRegistration](../masstransit-configuration/iexecuteactivityregistration)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Matches(IActivityRegistration)**

```csharp
bool Matches(IActivityRegistration registration)
```

#### Parameters

`registration` [IActivityRegistration](../masstransit-configuration/iactivityregistration)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Matches(IFutureRegistration)**

```csharp
bool Matches(IFutureRegistration registration)
```

#### Parameters

`registration` [IFutureRegistration](../masstransit-configuration/ifutureregistration)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Matches(IEndpointRegistration)**

```csharp
bool Matches(IEndpointRegistration registration)
```

#### Parameters

`registration` [IEndpointRegistration](../masstransit-configuration/iendpointregistration)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
