---

title: IFutureFaultConfigurator<TFault, TInput>

---

# IFutureFaultConfigurator\<TFault, TInput\>

Namespace: MassTransit

```csharp
public interface IFutureFaultConfigurator<TFault, TInput>
```

#### Type Parameters

`TFault`<br/>

`TInput`<br/>

## Methods

### **SetFaultedUsingFactory(EventMessageFactory\<FutureState, TInput, TFault\>)**

Fault the future using the specified factory method

```csharp
void SetFaultedUsingFactory(EventMessageFactory<FutureState, TInput, TFault> factoryMethod)
```

#### Parameters

`factoryMethod` [EventMessageFactory\<FutureState, TInput, TFault\>](../../masstransit-abstractions/masstransit/eventmessagefactory-3)<br/>
Returns the result

### **SetFaultedUsingFactory(AsyncEventMessageFactory\<FutureState, TInput, TFault\>)**

Fault the future using the specified factory method

```csharp
void SetFaultedUsingFactory(AsyncEventMessageFactory<FutureState, TInput, TFault> factoryMethod)
```

#### Parameters

`factoryMethod` [AsyncEventMessageFactory\<FutureState, TInput, TFault\>](../../masstransit-abstractions/masstransit/asynceventmessagefactory-3)<br/>
Returns the result

### **SetFaultedUsingInitializer(InitializerValueProvider\<TInput\>)**

Fault the future using the a message initializer. The initiating command is also used to initialize
 result properties prior to apply the values specified.

```csharp
void SetFaultedUsingInitializer(InitializerValueProvider<TInput> valueProvider)
```

#### Parameters

`valueProvider` [InitializerValueProvider\<TInput\>](../masstransit/initializervalueprovider-1)<br/>
Returns an object of values to initialize the result
