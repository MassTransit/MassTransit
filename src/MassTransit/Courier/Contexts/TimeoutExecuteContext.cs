namespace MassTransit.Courier.Contexts
{
    using System;
    using System.Collections.Generic;
    using System.Threading;


    public class TimeoutExecuteContext<TArguments> :
        TimeoutCourierContextProxy,
        ExecuteContext<TArguments>
        where TArguments : class
    {
        readonly ExecuteContext<TArguments> _context;

        public TimeoutExecuteContext(ExecuteContext<TArguments> context, CancellationToken cancellationToken)
            : base(context, cancellationToken)
        {
            _context = context;
        }

        ExecutionResult ExecuteContext.Completed()
        {
            return _context.Completed();
        }

        ExecutionResult ExecuteContext.CompletedWithVariables(IEnumerable<KeyValuePair<string, object>> variables)
        {
            return _context.CompletedWithVariables(variables);
        }

        ExecutionResult ExecuteContext.CompletedWithVariables(object variables)
        {
            return _context.CompletedWithVariables(variables);
        }

        ExecutionResult ExecuteContext.Completed<TLog>(TLog log)
        {
            return _context.Completed(log);
        }

        ExecutionResult ExecuteContext.Completed<TLog>(object logValues)
        {
            return _context.Completed<TLog>(logValues);
        }

        ExecutionResult ExecuteContext.CompletedWithVariables<TLog>(TLog log, object variables)
        {
            return _context.CompletedWithVariables(log, variables);
        }

        ExecutionResult ExecuteContext.CompletedWithVariables<TLog>(object logValues, object variables)
        {
            return _context.CompletedWithVariables<TLog>(logValues, variables);
        }

        ExecutionResult ExecuteContext.CompletedWithVariables<TLog>(TLog log, IEnumerable<KeyValuePair<string, object>> variables)
        {
            return _context.CompletedWithVariables(log, variables);
        }

        ExecutionResult ExecuteContext.ReviseItinerary(Action<ItineraryBuilder> buildItinerary)
        {
            return _context.ReviseItinerary(buildItinerary);
        }

        ExecutionResult ExecuteContext.ReviseItinerary<TLog>(TLog log, Action<ItineraryBuilder> buildItinerary)
        {
            return _context.ReviseItinerary(log, buildItinerary);
        }

        ExecutionResult ExecuteContext.ReviseItinerary<TLog>(TLog log, object variables, Action<ItineraryBuilder> buildItinerary)
        {
            return _context.ReviseItinerary(log, variables, buildItinerary);
        }

        ExecutionResult ExecuteContext.ReviseItinerary<TLog>(TLog log, IEnumerable<KeyValuePair<string, object>> variables,
            Action<ItineraryBuilder> buildItinerary)
        {
            return _context.ReviseItinerary(log, variables, buildItinerary);
        }

        ExecutionResult ExecuteContext.Terminate()
        {
            return _context.Terminate();
        }

        ExecutionResult ExecuteContext.Terminate(object variables)
        {
            return _context.Terminate(variables);
        }

        ExecutionResult ExecuteContext.Terminate(IEnumerable<KeyValuePair<string, object>> variables)
        {
            return _context.Terminate(variables);
        }

        ExecutionResult ExecuteContext.Faulted()
        {
            return _context.Faulted();
        }

        ExecutionResult ExecuteContext.Faulted(Exception exception)
        {
            return _context.Faulted(exception);
        }

        ExecutionResult ExecuteContext.FaultedWithVariables(Exception exception, object variables)
        {
            return _context.FaultedWithVariables(exception, variables);
        }

        ExecutionResult ExecuteContext.FaultedWithVariables(Exception exception, IEnumerable<KeyValuePair<string, object>> variables)
        {
            return _context.FaultedWithVariables(exception, variables);
        }

        ExecutionResult ExecuteContext.Result
        {
            get => _context.Result;
            set => _context.Result = value;
        }

        TArguments ExecuteContext<TArguments>.Arguments => _context.Arguments;

        ExecuteActivityContext<TActivity, TArguments> ExecuteContext<TArguments>.CreateActivityContext<TActivity>(TActivity activity)
        {
            return new HostExecuteActivityContext<TActivity, TArguments>(activity, this);
        }
    }
}
