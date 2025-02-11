---

title: IFutureResultConfigurator<TResult>

---

# IFutureResultConfigurator\<TResult\>

Namespace: MassTransit

```csharp
public interface IFutureResultConfigurator<TResult>
```

#### Type Parameters

`TResult`<br/>

## Methods

### **SetCompletedUsingFactory(EventMessageFactory\<FutureState, TResult\>)**

Complete the future using the specified factory method

```csharp
void SetCompletedUsingFactory(EventMessageFactory<FutureState, TResult> factoryMethod)
```

#### Parameters

`factoryMethod` [EventMessageFactory\<FutureState, TResult\>](../../masstransit-abstractions/masstransit/eventmessagefactory-2)<br/>
Returns the result

### **SetCompletedUsingFactory(AsyncEventMessageFactory\<FutureState, TResult\>)**

Complete the future using the specified factory method

```csharp
void SetCompletedUsingFactory(AsyncEventMessageFactory<FutureState, TResult> factoryMethod)
```

#### Parameters

`factoryMethod` [AsyncEventMessageFactory\<FutureState, TResult\>](../../masstransit-abstractions/masstransit/asynceventmessagefactory-2)<br/>
Returns the result

### **SetCompletedUsingInitializer(InitializerValueProvider)**

Complete the future using the a message initializer. The initiating command is also used to initialize
 result properties prior to apply the values specified.

```csharp
void SetCompletedUsingInitializer(InitializerValueProvider valueProvider)
```

#### Parameters

`valueProvider` [InitializerValueProvider](../masstransit/initializervalueprovider)<br/>
Returns an object of values to initialize the result
