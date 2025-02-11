---

title: FutureResultConfigurator<TCommand, TResult>

---

# FutureResultConfigurator\<TCommand, TResult\>

Namespace: MassTransit.Configuration

```csharp
public class FutureResultConfigurator<TCommand, TResult> : IFutureResultConfigurator<TResult>
```

#### Type Parameters

`TCommand`<br/>

`TResult`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [FutureResultConfigurator\<TCommand, TResult\>](../masstransit-configuration/futureresultconfigurator-2)<br/>
Implements [IFutureResultConfigurator\<TResult\>](../masstransit/ifutureresultconfigurator-1)

## Constructors

### **FutureResultConfigurator(FutureResult\<TCommand, TResult\>)**

```csharp
public FutureResultConfigurator(FutureResult<TCommand, TResult> result)
```

#### Parameters

`result` [FutureResult\<TCommand, TResult\>](../masstransit-futures/futureresult-2)<br/>

## Methods

### **SetCompletedUsingFactory(EventMessageFactory\<FutureState, TResult\>)**

```csharp
public void SetCompletedUsingFactory(EventMessageFactory<FutureState, TResult> factoryMethod)
```

#### Parameters

`factoryMethod` [EventMessageFactory\<FutureState, TResult\>](../../masstransit-abstractions/masstransit/eventmessagefactory-2)<br/>

### **SetCompletedUsingFactory(AsyncEventMessageFactory\<FutureState, TResult\>)**

```csharp
public void SetCompletedUsingFactory(AsyncEventMessageFactory<FutureState, TResult> factoryMethod)
```

#### Parameters

`factoryMethod` [AsyncEventMessageFactory\<FutureState, TResult\>](../../masstransit-abstractions/masstransit/asynceventmessagefactory-2)<br/>

### **SetCompletedUsingInitializer(InitializerValueProvider)**

```csharp
public void SetCompletedUsingInitializer(InitializerValueProvider valueProvider)
```

#### Parameters

`valueProvider` [InitializerValueProvider](../masstransit/initializervalueprovider)<br/>
