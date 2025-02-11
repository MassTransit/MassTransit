---

title: Schedule<TSaga>

---

# Schedule\<TSaga\>

Namespace: MassTransit

Holds the state of a scheduled message

```csharp
public interface Schedule<TSaga>
```

#### Type Parameters

`TSaga`<br/>

## Properties

### **Name**

The name of the scheduled message

```csharp
public abstract string Name { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Methods

### **GetDelay(BehaviorContext\<TSaga\>)**

Returns the delay, given the instance, for the scheduled message

```csharp
TimeSpan GetDelay(BehaviorContext<TSaga> context)
```

#### Parameters

`context` [BehaviorContext\<TSaga\>](../masstransit/behaviorcontext-1)<br/>

#### Returns

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **GetTokenId(TSaga)**

Return the TokenId for the instance

```csharp
Nullable<Guid> GetTokenId(TSaga instance)
```

#### Parameters

`instance` TSaga<br/>

#### Returns

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **SetTokenId(TSaga, Nullable\<Guid\>)**

Set the token ID on the Instance

```csharp
void SetTokenId(TSaga instance, Nullable<Guid> tokenId)
```

#### Parameters

`instance` TSaga<br/>

`tokenId` [Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>
