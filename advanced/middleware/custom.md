# Creating your own middleware

Middleware components are configured using extension methods, to make them easy to discover.
By default, a middleware configuration method should start with Use.

An example middleware component that would log exceptions to the console is shown below.

```csharp
Bus.Factory.CreateUsingInMemory(cfg =>
{
    cfg.UseExceptionLogger();
});
```

The extension method creates the pipe specification for the middleware component, which can
be added to any pipe. For a component on the message consumption pipeline, use `ConsumeContext`
instead of any `PipeContext`.

```csharp
public static class ExampleMiddlewareConfiguratorExtensions
{
    public static void UseExceptionLogger<T>(this IPipeConfigurator<T> configurator)
        where T : class, PipeContext
    {
        configurator.AddPipeSpecification(new ExceptionLoggerSpecification<T>());
    }
}
```

The pipe specification is a class that adds the filter to the pipeline. Additional logic
can be included, such as configuring optional settings, etc. using a closure syntax similar
to the other configuration classes in MassTransit.

```csharp
public class ExceptionLoggerSpecification<T> :
    IPipeSpecification<T>
    where T : class, PipeContext
{
    public IEnumerable<ValidationResult> Validate()
    {
        return Enumerable.Empty<ValidationResult>();
    }

    public void Apply(IPipeBuilder<T> builder)
    {
        builder.AddFilter(new ExceptionLoggerFilter<T>());
    }
}
```

Finally, the middleware component itself is a filter connected to the pipeline (inline). All filters
have absolute and complete control of the execution context and flow of the message. Pipelines are
entirely asynchronous, and expect that asynchronous operations will be performed.

<div class="alert alert-warning">
<b>Note:</b>
    Do not use legacy constructs such as .Wait, .Result, or .WaitAll() as these can cause blocking
    in the message pipeline. While they might work in same cases, you've been warned!
</div>

```csharp
public class ExceptionLoggerFilter<T> :
    IFilter<T>
    where T : class, PipeContext
{
    long _exceptionCount;
    long _successCount;
    long _attemptCount;

    public void Probe(ProbeContext context)
    {
        var scope = context.CreateFilterScope("exceptionLogger");
        scope.Add("attempted", _attemptCount);
        scope.Add("succeeded", _successCount);
        scope.Add("faulted", _exceptionCount);
    }

    public async Task Send(T context, IPipe<T> next)
    {
        try
        {
            Interlocked.Increment(ref _attemptCount);
            await next.Send(context);
            Interlocked.Increment(ref _successCount);
        }
        catch (Exception ex)
        {
            Interlocked.Increment(ref _exceptionCount);
            await Console.Out.WriteLineAsync($"An exception occurred: {ex.Message}");
            // propagate the exception up the call stack
            throw;
        }
    }
}
```
