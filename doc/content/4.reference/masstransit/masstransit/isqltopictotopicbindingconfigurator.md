---

title: ISqlTopicToTopicBindingConfigurator

---

# ISqlTopicToTopicBindingConfigurator

Namespace: MassTransit

```csharp
public interface ISqlTopicToTopicBindingConfigurator : ISqlTopicSubscriptionConfigurator, ISqlTopicConfigurator
```

Implements [ISqlTopicSubscriptionConfigurator](../masstransit/isqltopicsubscriptionconfigurator), [ISqlTopicConfigurator](../masstransit/isqltopicconfigurator)

## Methods

### **Subscribe(String, Action\<ISqlTopicToTopicBindingConfigurator\>)**

Creates a subscription between two topics

```csharp
void Subscribe(string topicName, Action<ISqlTopicToTopicBindingConfigurator> configure)
```

#### Parameters

`topicName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
Topic name of the new exchange

`configure` [Action\<ISqlTopicToTopicBindingConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
Configuration for new exchange and how to bind to it
