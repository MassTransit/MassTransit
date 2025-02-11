---

title: DependencyInjectionTransactionExtensions

---

# DependencyInjectionTransactionExtensions

Namespace: MassTransit

```csharp
public static class DependencyInjectionTransactionExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DependencyInjectionTransactionExtensions](../masstransit/dependencyinjectiontransactionextensions)

## Methods

### **AddTransactionalEnlistmentBus(IBusRegistrationConfigurator)**

Adds [ITransactionalBus](../masstransit-transactions/itransactionalbus) to the container with singleton lifetime, which can be used instead of  to enlist
 published/sent messages in the current transaction. It isn't truly transactional, but delays the messages until
 the transaction being to commit. This has a very limited purpose and is not meant for general use.

```csharp
public static void AddTransactionalEnlistmentBus(IBusRegistrationConfigurator busConfigurator)
```

#### Parameters

`busConfigurator` [IBusRegistrationConfigurator](../masstransit/ibusregistrationconfigurator)<br/>

### **AddTransactionalEnlistmentBus\<TBus\>(IBusRegistrationConfigurator\<TBus\>)**

Adds [ITransactionalBus](../masstransit-transactions/itransactionalbus) to the container with singleton lifetime, which can be used instead of  to enlist
 published/sent messages in the current transaction. It isn't truly transactional, but delays the messages until
 the transaction being to commit. This has a very limited purpose and is not meant for general use.

```csharp
public static void AddTransactionalEnlistmentBus<TBus>(IBusRegistrationConfigurator<TBus> busConfigurator)
```

#### Type Parameters

`TBus`<br/>

#### Parameters

`busConfigurator` [IBusRegistrationConfigurator\<TBus\>](../masstransit/ibusregistrationconfigurator-1)<br/>

### **AddTransactionalBus(IBusRegistrationConfigurator)**

Adds [ITransactionalBus](../masstransit-transactions/itransactionalbus) to the container with scoped lifetime, which can be used to release the messages to the bus
 immediately after a transaction commit. This has a very limited purpose and is not meant for general use.
 It is recommended this is scoped within a unit of work (e.g. Http Request)

```csharp
public static void AddTransactionalBus(IBusRegistrationConfigurator busConfigurator)
```

#### Parameters

`busConfigurator` [IBusRegistrationConfigurator](../masstransit/ibusregistrationconfigurator)<br/>

### **AddTransactionalBus\<TBus\>(IBusRegistrationConfigurator\<TBus\>)**

Adds [ITransactionalBus](../masstransit-transactions/itransactionalbus) to the container with scoped lifetime, which can be used to release the messages to the bus
 immediately after a transaction commit. This has a very limited purpose and is not meant for general use.
 It is recommended this is scoped within a unit of work (e.g. Http Request)

```csharp
public static void AddTransactionalBus<TBus>(IBusRegistrationConfigurator<TBus> busConfigurator)
```

#### Type Parameters

`TBus`<br/>

#### Parameters

`busConfigurator` [IBusRegistrationConfigurator\<TBus\>](../masstransit/ibusregistrationconfigurator-1)<br/>
