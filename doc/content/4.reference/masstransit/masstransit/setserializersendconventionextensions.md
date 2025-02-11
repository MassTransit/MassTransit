---

title: SetSerializerSendConventionExtensions

---

# SetSerializerSendConventionExtensions

Namespace: MassTransit

```csharp
public static class SetSerializerSendConventionExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SetSerializerSendConventionExtensions](../masstransit/setserializersendconventionextensions)

## Methods

### **UseSerializer\<T\>(IMessageSendTopologyConfigurator\<T\>, ContentType)**

Use the message serializer identified by the specified content type to serialize messages of this type

```csharp
public static void UseSerializer<T>(IMessageSendTopologyConfigurator<T> configurator, ContentType contentType)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`configurator` [IMessageSendTopologyConfigurator\<T\>](../../masstransit-abstractions/masstransit/imessagesendtopologyconfigurator-1)<br/>

`contentType` ContentType<br/>

### **UseSerializer\<T\>(IMessageSendTopologyConfigurator\<T\>, String)**

Use the message serializer identified by the specified content type to serialize messages of this type

```csharp
public static void UseSerializer<T>(IMessageSendTopologyConfigurator<T> configurator, string contentType)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`configurator` [IMessageSendTopologyConfigurator\<T\>](../../masstransit-abstractions/masstransit/imessagesendtopologyconfigurator-1)<br/>

`contentType` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
