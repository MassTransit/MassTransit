#nullable enable
namespace MassTransit.Middleware.InMemoryOutbox;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Util;


public class InMemoryOutboxDeferredMethodCollection
{
    readonly Task? _clearToSend;
    readonly List<InMemoryOutboxDeferredMethod> _pendingMethods;

    public InMemoryOutboxDeferredMethodCollection(Task? clearToSend = null)
    {
        _clearToSend = clearToSend;
        _pendingMethods = [];
    }

    public Task Add(Func<Task> method)
    {
        if (_clearToSend?.IsCompleted ?? false)
            return method();

        var executionContext = ExecutionContext.Capture();

        lock (_pendingMethods)
        {
            _pendingMethods.Add(new InMemoryOutboxDeferredMethod(executionContext, method));

            return Task.CompletedTask;
        }
    }

    public async Task Execute(bool concurrent)
    {
        InMemoryOutboxDeferredMethod[] pendingActions;
        lock (_pendingMethods)
            pendingActions = _pendingMethods.ToArray();

        try
        {
            if (pendingActions.Length > 0)
            {
                if (concurrent)
                {
                    var collection = new PendingTaskCollection(pendingActions.Length);

                    collection.Add(pendingActions.Select(method => method.Run()));

                    await collection.Completed().ConfigureAwait(false);
                }
                else
                {
                    foreach (var method in pendingActions)
                    {
                        var task = method.Run();
                        if (task != null)
                            await task.ConfigureAwait(false);
                    }
                }
            }
        }
        finally
        {
            foreach (var deferredMethod in pendingActions)
                deferredMethod.Dispose();
        }
    }

    public async Task Discard()
    {
        InMemoryOutboxDeferredMethod[] pendingMethods;
        lock (_pendingMethods)
        {
            pendingMethods = _pendingMethods.ToArray();
            _pendingMethods.Clear();
        }

        foreach (var method in pendingMethods)
            method.Dispose();
    }
}
