---

title: IEventCorrelationConfigurator<TSaga, TMessage>

---

# IEventCorrelationConfigurator\<TSaga, TMessage\>

Namespace: MassTransit

```csharp
public interface IEventCorrelationConfigurator<TSaga, TMessage>
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

## Properties

### **InsertOnInitial**

If set to true, the state machine suggests that the saga instance be inserted blinding prior to the get/lock
 using a weaker isolation level. This prevents range locks in the database from slowing inserts.

```csharp
public abstract bool InsertOnInitial { set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **ReadOnly**

If set to true, changes to the saga instance will not be saved to the repository. Note that the in-memory saga repository
 does not support read-only since the changes are made directly to the saga instance.

```csharp
public abstract bool ReadOnly { set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **ConfigureConsumeTopology**

If set to false, the event type will not be configured as part of the broker topology

```csharp
public abstract bool ConfigureConsumeTopology { set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

## Methods

### **CorrelateById(Func\<ConsumeContext\<TMessage\>, Guid\>)**

Correlate to the saga instance by CorrelationId, using the id from the event data

```csharp
IEventCorrelationConfigurator<TSaga, TMessage> CorrelateById(Func<ConsumeContext<TMessage>, Guid> selector)
```

#### Parameters

`selector` [Func\<ConsumeContext\<TMessage\>, Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
Returns the CorrelationId from the event data

#### Returns

[IEventCorrelationConfigurator\<TSaga, TMessage\>](../masstransit/ieventcorrelationconfigurator-2)<br/>

### **CorrelateById\<T\>(Expression\<Func\<TSaga, T\>\>, Func\<ConsumeContext\<TMessage\>, T\>)**

Correlate to the saga instance by a single value property, matched to the property value of the message

```csharp
IEventCorrelationConfigurator<TSaga, TMessage> CorrelateById<T>(Expression<Func<TSaga, T>> propertyExpression, Func<ConsumeContext<TMessage>, T> selector)
```

#### Type Parameters

`T`<br/>

#### Parameters

`propertyExpression` Expression\<Func\<TSaga, T\>\><br/>
The instance property

`selector` [Func\<ConsumeContext\<TMessage\>, T\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
The identifier selector for the message

#### Returns

[IEventCorrelationConfigurator\<TSaga, TMessage\>](../masstransit/ieventcorrelationconfigurator-2)<br/>

### **CorrelateBy\<T\>(Expression\<Func\<TSaga, Nullable\<T\>\>\>, Func\<ConsumeContext\<TMessage\>, Nullable\<T\>\>)**

Correlate to the saga instance by a single property, matched to the property value of the message

```csharp
IEventCorrelationConfigurator<TSaga, TMessage> CorrelateBy<T>(Expression<Func<TSaga, Nullable<T>>> propertyExpression, Func<ConsumeContext<TMessage>, Nullable<T>> selector)
```

#### Type Parameters

`T`<br/>

#### Parameters

`propertyExpression` Expression\<Func\<TSaga, Nullable\<T\>\>\><br/>
The instance property

`selector` [Func\<ConsumeContext\<TMessage\>, Nullable\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[IEventCorrelationConfigurator\<TSaga, TMessage\>](../masstransit/ieventcorrelationconfigurator-2)<br/>

### **CorrelateBy\<T\>(Expression\<Func\<TSaga, T\>\>, Func\<ConsumeContext\<TMessage\>, T\>)**

Correlate to the saga instance by a single property, matched to the property value of the message

```csharp
IEventCorrelationConfigurator<TSaga, TMessage> CorrelateBy<T>(Expression<Func<TSaga, T>> propertyExpression, Func<ConsumeContext<TMessage>, T> selector)
```

#### Type Parameters

`T`<br/>

#### Parameters

`propertyExpression` Expression\<Func\<TSaga, T\>\><br/>
The instance property

`selector` [Func\<ConsumeContext\<TMessage\>, T\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[IEventCorrelationConfigurator\<TSaga, TMessage\>](../masstransit/ieventcorrelationconfigurator-2)<br/>

### **SelectId(Func\<ConsumeContext\<TMessage\>, Guid\>)**

When creating a new saga instance, initialize the saga CorrelationId with the id from the event data

```csharp
IEventCorrelationConfigurator<TSaga, TMessage> SelectId(Func<ConsumeContext<TMessage>, Guid> selector)
```

#### Parameters

`selector` [Func\<ConsumeContext\<TMessage\>, Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
Returns the CorrelationId from the event data

#### Returns

[IEventCorrelationConfigurator\<TSaga, TMessage\>](../masstransit/ieventcorrelationconfigurator-2)<br/>

### **CorrelateBy(Expression\<Func\<TSaga, ConsumeContext\<TMessage\>, Boolean\>\>)**

Specify the correlation expression for the event

```csharp
IEventCorrelationConfigurator<TSaga, TMessage> CorrelateBy(Expression<Func<TSaga, ConsumeContext<TMessage>, bool>> correlationExpression)
```

#### Parameters

`correlationExpression` Expression\<Func\<TSaga, ConsumeContext\<TMessage\>, Boolean\>\><br/>

#### Returns

[IEventCorrelationConfigurator\<TSaga, TMessage\>](../masstransit/ieventcorrelationconfigurator-2)<br/>

### **SetSagaFactory(SagaFactoryMethod\<TSaga, TMessage\>)**

Creates a new instance of the saga, and if appropriate, pre-inserts the saga instance to the database. If the saga already exists, any
 exceptions from the insert are suppressed and processing continues normally.

```csharp
IEventCorrelationConfigurator<TSaga, TMessage> SetSagaFactory(SagaFactoryMethod<TSaga, TMessage> factoryMethod)
```

#### Parameters

`factoryMethod` [SagaFactoryMethod\<TSaga, TMessage\>](../masstransit/sagafactorymethod-2)<br/>
The factory method for the saga

#### Returns

[IEventCorrelationConfigurator\<TSaga, TMessage\>](../masstransit/ieventcorrelationconfigurator-2)<br/>

### **OnMissingInstance(Func\<IMissingInstanceConfigurator\<TSaga, TMessage\>, IPipe\<ConsumeContext\<TMessage\>\>\>)**

If an event is consumed that is not matched to an existing saga instance, discard the event without throwing an exception.
 The default behavior is to throw an exception, which moves the event into the error queue for later processing

```csharp
IEventCorrelationConfigurator<TSaga, TMessage> OnMissingInstance(Func<IMissingInstanceConfigurator<TSaga, TMessage>, IPipe<ConsumeContext<TMessage>>> getBehavior)
```

#### Parameters

`getBehavior` [Func\<IMissingInstanceConfigurator\<TSaga, TMessage\>, IPipe\<ConsumeContext\<TMessage\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
The configuration call to specify the behavior on missing instance

#### Returns

[IEventCorrelationConfigurator\<TSaga, TMessage\>](../masstransit/ieventcorrelationconfigurator-2)<br/>
