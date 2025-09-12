namespace MassTransit.Middleware.InMemoryOutbox;

using System;
using System.Threading;
using System.Threading.Tasks;


public class InMemoryOutboxDeferredMethod :
    IDisposable
{
    readonly Func<Task> _method;
    ExecutionContext _executionContext;

    public InMemoryOutboxDeferredMethod(ExecutionContext executionContext, Func<Task> method)
    {
        _executionContext = executionContext;
        _method = method;
    }

    public void Dispose()
    {
        _executionContext?.Dispose();
    }

    public async Task Run()
    {
        try
        {
            using var ec = _executionContext.CreateCopy();

            Task task = null;

            ExecutionContext.Run(ec, _ => task = _method(), null);

            await task.ConfigureAwait(false);
        }
        finally
        {
            _executionContext?.Dispose();
            _executionContext = null;
        }
    }
}
