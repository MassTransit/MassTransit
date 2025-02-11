---

title: SendHeadersExtensions

---

# SendHeadersExtensions

Namespace: MassTransit

```csharp
public static class SendHeadersExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SendHeadersExtensions](../masstransit/sendheadersextensions)

## Methods

### **CopyFrom(SendHeaders, Headers)**

Copy the headers from an existing header collection into the  header collection

```csharp
public static void CopyFrom(SendHeaders sendHeaders, Headers headers)
```

#### Parameters

`sendHeaders` [SendHeaders](../masstransit/sendheaders)<br/>

`headers` [Headers](../masstransit/headers)<br/>
The source header collection

#### Exceptions

[ArgumentNullException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentnullexception)<br/>

### **CopyFrom\<T\>(ITransportSetHeaderAdapter\<T\>, IDictionary\<String, T\>, Headers)**

Copy the headers from an existing header collection into the  header collection

```csharp
public static void CopyFrom<T>(ITransportSetHeaderAdapter<T> adapter, IDictionary<string, T> sendHeaders, Headers headers)
```

#### Type Parameters

`T`<br/>

#### Parameters

`adapter` [ITransportSetHeaderAdapter\<T\>](../masstransit-transports/itransportsetheaderadapter-1)<br/>
The dictionary adapter for setting headers

`sendHeaders` [IDictionary\<String, T\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>
The send header collection

`headers` [Headers](../masstransit/headers)<br/>
The source header collection

#### Exceptions

[ArgumentNullException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentnullexception)<br/>
