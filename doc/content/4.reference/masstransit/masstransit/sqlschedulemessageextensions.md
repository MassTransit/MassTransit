---

title: SqlScheduleMessageExtensions

---

# SqlScheduleMessageExtensions

Namespace: MassTransit

```csharp
public static class SqlScheduleMessageExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SqlScheduleMessageExtensions](../masstransit/sqlschedulemessageextensions)

## Methods

### **UseDbMessageScheduler(IBusFactoryConfigurator)**

#### Caution

Use the renamed UseSqlMessageScheduler instead

---

Uses the SQL transport's built-in message scheduler

```csharp
public static void UseDbMessageScheduler(IBusFactoryConfigurator configurator)
```

#### Parameters

`configurator` [IBusFactoryConfigurator](../../masstransit-abstractions/masstransit/ibusfactoryconfigurator)<br/>

### **UseSqlMessageScheduler(IBusFactoryConfigurator)**

Uses the SQL transport's built-in message scheduler

```csharp
public static void UseSqlMessageScheduler(IBusFactoryConfigurator configurator)
```

#### Parameters

`configurator` [IBusFactoryConfigurator](../../masstransit-abstractions/masstransit/ibusfactoryconfigurator)<br/>

### **AddSqlMessageScheduler(IBusRegistrationConfigurator)**

Add a  to the container that uses the SQL Transport message enqueue time to schedule messages.

```csharp
public static void AddSqlMessageScheduler(IBusRegistrationConfigurator configurator)
```

#### Parameters

`configurator` [IBusRegistrationConfigurator](../masstransit/ibusregistrationconfigurator)<br/>

### **AddSqlMessageScheduler\<TBus\>(IBusRegistrationConfigurator\<TBus\>)**

Add a  to the container that uses the SQL Transport message enqueue time to schedule messages.

```csharp
public static void AddSqlMessageScheduler<TBus>(IBusRegistrationConfigurator<TBus> configurator)
```

#### Type Parameters

`TBus`<br/>

#### Parameters

`configurator` [IBusRegistrationConfigurator\<TBus\>](../masstransit/ibusregistrationconfigurator-1)<br/>
