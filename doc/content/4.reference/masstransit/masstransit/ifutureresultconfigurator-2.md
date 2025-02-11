---

title: IFutureResultConfigurator<TResult, TInput>

---

# IFutureResultConfigurator\<TResult, TInput\>

Namespace: MassTransit

```csharp
public interface IFutureResultConfigurator<TResult, TInput>
```

#### Type Parameters

`TResult`<br/>

`TInput`<br/>

## Methods

### **SetCompletedUsingFactory(EventMessageFactory\<FutureState, TInput, TResult\>)**

Complete the future using the specified factory method

```csharp
void SetCompletedUsingFactory(EventMessageFactory<FutureState, TInput, TResult> factoryMethod)
```

#### Parameters

`factoryMethod` [EventMessageFactory\<FutureState, TInput, TResult\>](../../masstransit-abstractions/masstransit/eventmessagefactory-3)<br/>
Returns the result

### **SetCompletedUsingFactory(AsyncEventMessageFactory\<FutureState, TInput, TResult\>)**

Complete the future using the specified factory method

```csharp
void SetCompletedUsingFactory(AsyncEventMessageFactory<FutureState, TInput, TResult> factoryMethod)
```

#### Parameters

`factoryMethod` [AsyncEventMessageFactory\<FutureState, TInput, TResult\>](../../masstransit-abstractions/masstransit/asynceventmessagefactory-3)<br/>
Returns the result

### **SetCompletedUsingInitializer(InitializerValueProvider\<TInput\>)**

Complete the future using the a message initializer. The initiating command is also used to initialize
 result properties prior to apply the values specified.

```csharp
void SetCompletedUsingInitializer(InitializerValueProvider<TInput> valueProvider)
```

#### Parameters

`valueProvider` [InitializerValueProvider\<TInput\>](../masstransit/initializervalueprovider-1)<br/>
Returns an object of values to initialize the result
