---

title: CircuitBreakerConfigurationExtensions

---

# CircuitBreakerConfigurationExtensions

Namespace: MassTransit

```csharp
public static class CircuitBreakerConfigurationExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [CircuitBreakerConfigurationExtensions](../masstransit/circuitbreakerconfigurationextensions)

## Methods

### **UseCircuitBreaker\<T\>(IPipeConfigurator\<T\>, Action\<ICircuitBreakerConfigurator\<T\>\>)**

Puts a circuit breaker in the pipe, which can automatically prevent the flow of messages to the consumer
 when the circuit breaker is opened.

```csharp
public static void UseCircuitBreaker<T>(IPipeConfigurator<T> configurator, Action<ICircuitBreakerConfigurator<T>> configure)
```

#### Type Parameters

`T`<br/>
The pipe context type

#### Parameters

`configurator` [IPipeConfigurator\<T\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>

`configure` [Action\<ICircuitBreakerConfigurator\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
