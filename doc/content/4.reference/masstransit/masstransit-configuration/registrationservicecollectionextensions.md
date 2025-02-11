---

title: RegistrationServiceCollectionExtensions

---

# RegistrationServiceCollectionExtensions

Namespace: MassTransit.Configuration

```csharp
public static class RegistrationServiceCollectionExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RegistrationServiceCollectionExtensions](../masstransit-configuration/registrationservicecollectionextensions)

## Methods

### **RegisterSagaRepository\<TSaga, TContext, TConsumeContextFactory, TRepositoryContextFactory\>(IServiceCollection)**

```csharp
public static void RegisterSagaRepository<TSaga, TContext, TConsumeContextFactory, TRepositoryContextFactory>(IServiceCollection collection)
```

#### Type Parameters

`TSaga`<br/>

`TContext`<br/>

`TConsumeContextFactory`<br/>

`TRepositoryContextFactory`<br/>

#### Parameters

`collection` IServiceCollection<br/>

### **RegisterQuerySagaRepository\<TSaga, TQueryRepositoryContextFactory\>(IServiceCollection)**

```csharp
public static void RegisterQuerySagaRepository<TSaga, TQueryRepositoryContextFactory>(IServiceCollection collection)
```

#### Type Parameters

`TSaga`<br/>

`TQueryRepositoryContextFactory`<br/>

#### Parameters

`collection` IServiceCollection<br/>

### **RegisterLoadSagaRepository\<TSaga, TLoadRepositoryContextFactory\>(IServiceCollection)**

```csharp
public static void RegisterLoadSagaRepository<TSaga, TLoadRepositoryContextFactory>(IServiceCollection collection)
```

#### Type Parameters

`TSaga`<br/>

`TLoadRepositoryContextFactory`<br/>

#### Parameters

`collection` IServiceCollection<br/>

### **RemoveSagaRepositories(IServiceCollection)**

```csharp
internal static void RemoveSagaRepositories(IServiceCollection collection)
```

#### Parameters

`collection` IServiceCollection<br/>
