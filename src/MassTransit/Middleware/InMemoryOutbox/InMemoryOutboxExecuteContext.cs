namespace MassTransit.Middleware.InMemoryOutbox
{
    using System;
    using System.Collections.Generic;
    using Context;


    public class InMemoryOutboxExecuteContext<TArguments> :
        InMemoryOutboxCourierContextProxy,
        ExecuteContext<TArguments>
        where TArguments : class
    {
        readonly ExecuteContext<TArguments> _context;

        public InMemoryOutboxExecuteContext(ExecuteContext<TArguments> context)
            : base(context)
        {
            _context = context;
        }

        public ExecutionResult Completed()
        {
            return _context.Completed();
        }

        public ExecutionResult Completed(ConfigureCompletedActivityOptionsCallback callback)
        {
            return _context.Completed(callback);
        }

        public ExecutionResult CompletedWithVariables(IEnumerable<KeyValuePair<string, object>> variables)
        {
            return _context.CompletedWithVariables(variables);
        }

        public ExecutionResult CompletedWithVariables(object variables)
        {
            return _context.CompletedWithVariables(variables);
        }

        public ExecutionResult Completed<TLog>(TLog log)
            where TLog : class
        {
            return _context.Completed(log);
        }

        public ExecutionResult Completed<TLog>(TLog log, ConfigureCompletedActivityOptionsCallback callback)
            where TLog : class
        {
            return _context.Completed(log, callback);
        }

        public ExecutionResult Completed<TLog>(object logValues)
            where TLog : class
        {
            return _context.Completed<TLog>(logValues);
        }

        public ExecutionResult Completed<TLog>(object logValues, ConfigureCompletedActivityOptionsCallback callback)
            where TLog : class
        {
            return _context.Completed<TLog>(logValues, callback);
        }

        public ExecutionResult CompletedWithVariables<TLog>(TLog log, object variables)
            where TLog : class
        {
            return _context.CompletedWithVariables(log, variables);
        }

        public ExecutionResult CompletedWithVariables<TLog>(object logValues, object variables)
            where TLog : class
        {
            return _context.CompletedWithVariables<TLog>(logValues, variables);
        }

        public ExecutionResult CompletedWithVariables<TLog>(TLog log, IEnumerable<KeyValuePair<string, object>> variables)
            where TLog : class
        {
            return _context.CompletedWithVariables(log, variables);
        }

        public ExecutionResult ReviseItinerary(Action<IItineraryBuilder> buildItinerary)
        {
            return _context.ReviseItinerary(buildItinerary);
        }

        public ExecutionResult ReviseItinerary<TLog>(TLog log, Action<IItineraryBuilder> buildItinerary)
            where TLog : class
        {
            return _context.ReviseItinerary(log, buildItinerary);
        }

        public ExecutionResult ReviseItinerary<TLog>(TLog log, object variables, Action<IItineraryBuilder> buildItinerary)
            where TLog : class
        {
            return _context.ReviseItinerary(log, variables, buildItinerary);
        }

        public ExecutionResult ReviseItinerary<TLog>(TLog log, IEnumerable<KeyValuePair<string, object>> variables, Action<IItineraryBuilder> buildItinerary)
            where TLog : class
        {
            return _context.ReviseItinerary(log, variables, buildItinerary);
        }

        public ExecutionResult Terminate()
        {
            return _context.Terminate();
        }

        public ExecutionResult Terminate(object variables)
        {
            return _context.Terminate(variables);
        }

        public ExecutionResult Terminate(IEnumerable<KeyValuePair<string, object>> variables)
        {
            return _context.Terminate(variables);
        }

        public ExecutionResult Faulted()
        {
            return _context.Faulted();
        }

        public ExecutionResult Faulted(Exception exception)
        {
            return _context.Faulted(exception);
        }

        public ExecutionResult Faulted(Exception exception, ConfigureFaultedActivityOptionsCallback callback)
        {
            return _context.Faulted(exception, callback);
        }

        public ExecutionResult FaultedWithVariables(Exception exception, object variables)
        {
            return _context.FaultedWithVariables(exception, variables);
        }

        public ExecutionResult FaultedWithVariables(Exception exception, IEnumerable<KeyValuePair<string, object>> variables)
        {
            return _context.FaultedWithVariables(exception, variables);
        }

        public ExecutionResult Result
        {
            get => _context.Result;
            set => _context.Result = value;
        }

        public TArguments Arguments => _context.Arguments;

        public ExecuteActivityContext<TActivity, TArguments> CreateActivityContext<TActivity>(TActivity activity)
            where TActivity : class, IExecuteActivity<TArguments>
        {
            return new HostExecuteActivityContext<TActivity, TArguments>(activity, this);
        }
    }
}
