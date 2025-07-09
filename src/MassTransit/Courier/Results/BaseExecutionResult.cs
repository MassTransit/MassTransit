namespace MassTransit.Courier.Results;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Contracts;


abstract class BaseExecutionResult<TArguments> :
    ExecutionResult
    where TArguments : class
{
    protected BaseExecutionResult(ExecuteContext<TArguments> context, IRoutingSlipEventPublisher publisher, Activity activity, RoutingSlip routingSlip)
    {
        Context = context;
        Publisher = publisher;
        Activity = activity;
        RoutingSlip = routingSlip;
        Duration = Context.Elapsed;
    }

    protected ExecuteContext<TArguments> Context { get; }
    protected IRoutingSlipEventPublisher Publisher { get; }
    protected TimeSpan Duration { get; }

    protected RoutingSlip RoutingSlip { get; }

    protected Activity Activity { get; }

    protected IDictionary<string, object> Variables { get; private set; }

    public TimeSpan? Delay { get; set; }

    public abstract Task Evaluate();

    public virtual bool IsFaulted(out Exception exception)
    {
        exception = null;
        return false;
    }

    public void SetVariables(object variables)
    {
        IEnumerable<KeyValuePair<string, object>> dictionary = Context.SerializerContext.ToDictionary(variables);

        SetVariables(dictionary);
    }

    public void SetVariables(IEnumerable<KeyValuePair<string, object>> variables)
    {
        foreach (KeyValuePair<string, object> value in variables)
            SetVariable(value.Key, value.Value);
    }

    public void SetVariable(string key, object value)
    {
        Variables ??= new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        Variables[key] = value;
    }
}
