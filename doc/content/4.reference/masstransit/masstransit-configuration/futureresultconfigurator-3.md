---

title: FutureResultConfigurator<TCommand, TResult, TInput>

---

# FutureResultConfigurator\<TCommand, TResult, TInput\>

Namespace: MassTransit.Configuration

```csharp
public class FutureResultConfigurator<TCommand, TResult, TInput> : IFutureResultConfigurator<TResult, TInput>
```

#### Type Parameters

`TCommand`<br/>

`TResult`<br/>

`TInput`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [FutureResultConfigurator\<TCommand, TResult, TInput\>](../masstransit-configuration/futureresultconfigurator-3)<br/>
Implements [IFutureResultConfigurator\<TResult, TInput\>](../masstransit/ifutureresultconfigurator-2)

## Constructors

### **FutureResultConfigurator(FutureResult\<TCommand, TResult, TInput\>)**

```csharp
public FutureResultConfigurator(FutureResult<TCommand, TResult, TInput> result)
```

#### Parameters

`result` [FutureResult\<TCommand, TResult, TInput\>](../masstransit-futures/futureresult-3)<br/>

## Methods

### **SetCompletedUsingFactory(EventMessageFactory\<FutureState, TInput, TResult\>)**

```csharp
public void SetCompletedUsingFactory(EventMessageFactory<FutureState, TInput, TResult> factoryMethod)
```

#### Parameters

`factoryMethod` [EventMessageFactory\<FutureState, TInput, TResult\>](../../masstransit-abstractions/masstransit/eventmessagefactory-3)<br/>

### **SetCompletedUsingFactory(AsyncEventMessageFactory\<FutureState, TInput, TResult\>)**

```csharp
public void SetCompletedUsingFactory(AsyncEventMessageFactory<FutureState, TInput, TResult> factoryMethod)
```

#### Parameters

`factoryMethod` [AsyncEventMessageFactory\<FutureState, TInput, TResult\>](../../masstransit-abstractions/masstransit/asynceventmessagefactory-3)<br/>

### **SetCompletedUsingInitializer(InitializerValueProvider\<TInput\>)**

```csharp
public void SetCompletedUsingInitializer(InitializerValueProvider<TInput> valueProvider)
```

#### Parameters

`valueProvider` [InitializerValueProvider\<TInput\>](../masstransit/initializervalueprovider-1)<br/>
