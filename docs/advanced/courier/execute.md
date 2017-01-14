# Executing the routing slip

Once built, the routing slip is executed, which sends it to the first activity’s execute URI. 
To make it easy and to ensure that source information is included, an extension method on *IBus* 
is available, the usage of which is shown below.

```csharp
await bus.Execute(routingSlip);
```

It should be pointed out that if the address for the first activity is invalid or cannot be reached, 
an exception will be thrown by the *Execute* method.
