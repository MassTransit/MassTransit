---

title: ProbeResultBuilder

---

# ProbeResultBuilder

Namespace: MassTransit.Introspection

```csharp
public class ProbeResultBuilder : ScopeProbeContext, ProbeContext, IProbeResultBuilder
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ScopeProbeContext](../masstransit-introspection/scopeprobecontext) → [ProbeResultBuilder](../masstransit-introspection/proberesultbuilder)<br/>
Implements [ProbeContext](../../masstransit-abstractions/masstransit/probecontext), [IProbeResultBuilder](../masstransit-introspection/iproberesultbuilder)

## Constructors

### **ProbeResultBuilder(Guid, CancellationToken)**

```csharp
public ProbeResultBuilder(Guid probeId, CancellationToken cancellationToken)
```

#### Parameters

`probeId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

## Methods

### **Build()**

```csharp
public ProbeResult Build()
```

#### Returns

[ProbeResult](../masstransit-introspection/proberesult)<br/>
