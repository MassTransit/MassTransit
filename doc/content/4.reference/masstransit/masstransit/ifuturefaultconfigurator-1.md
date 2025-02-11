---

title: IFutureFaultConfigurator<TFault>

---

# IFutureFaultConfigurator\<TFault\>

Namespace: MassTransit

```csharp
public interface IFutureFaultConfigurator<TFault>
```

#### Type Parameters

`TFault`<br/>

## Methods

### **SetFaultedUsingFactory(EventMessageFactory\<FutureState, TFault\>)**

Fault the future using the specified factory method

```csharp
void SetFaultedUsingFactory(EventMessageFactory<FutureState, TFault> factoryMethod)
```

#### Parameters

`factoryMethod` [EventMessageFactory\<FutureState, TFault\>](../../masstransit-abstractions/masstransit/eventmessagefactory-2)<br/>
Returns the result

### **SetFaultedUsingFactory(AsyncEventMessageFactory\<FutureState, TFault\>)**

Fault the future using the specified factory method

```csharp
void SetFaultedUsingFactory(AsyncEventMessageFactory<FutureState, TFault> factoryMethod)
```

#### Parameters

`factoryMethod` [AsyncEventMessageFactory\<FutureState, TFault\>](../../masstransit-abstractions/masstransit/asynceventmessagefactory-2)<br/>
Returns the result

### **SetFaultedUsingInitializer(InitializerValueProvider)**

Fault the future using the a message initializer. The initiating command is also used to initialize
 result properties prior to apply the values specified.

```csharp
void SetFaultedUsingInitializer(InitializerValueProvider valueProvider)
```

#### Parameters

`valueProvider` [InitializerValueProvider](../masstransit/initializervalueprovider)<br/>
Returns an object of values to initialize the result
