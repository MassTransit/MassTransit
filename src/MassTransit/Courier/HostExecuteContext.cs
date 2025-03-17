namespace MassTransit.Courier
{
    using System;
    using System.Collections.Generic;
    using Context;
    using Contracts;
    using Results;


    public class HostExecuteContext<TArguments> :
        BaseCourierContext,
        ExecuteContext<TArguments>
        where TArguments : class
    {
        readonly Activity _activity;
        readonly Uri _compensationAddress;

        public HostExecuteContext(Uri compensationAddress, ConsumeContext<RoutingSlip> context)
            : base(context)
        {
            _compensationAddress = compensationAddress;

            if (RoutingSlip.Itinerary.Count == 0)
                throw new ArgumentException("The routingSlip must contain at least one activity");

            _activity = RoutingSlip.Itinerary[0];
            Arguments = RoutingSlip.GetActivityArguments<TArguments>();
        }

        public override string ActivityName => _activity.Name;
        public TArguments Arguments { get; }

        public ExecuteActivityContext<TActivity, TArguments> CreateActivityContext<TActivity>(TActivity activity)
            where TActivity : class, IExecuteActivity<TArguments>
        {
            return new HostExecuteActivityContext<TActivity, TArguments>(activity, this);
        }

        public ExecutionResult Result { get; set; }

        public ExecutionResult Completed()
        {
            return new CompletedExecutionResult<TArguments>(this, Publisher, _activity, RoutingSlip, _compensationAddress);
        }

        public ExecutionResult Completed(ConfigureCompletedActivityOptionsCallback callback)
        {
            var result = new CompletedExecutionResult<TArguments>(this, Publisher, _activity, RoutingSlip, _compensationAddress);

            callback(result);

            return result;
        }

        public ExecutionResult Completed<TLog>(TLog log)
            where TLog : class
        {
            if (log == null)
                throw new ArgumentNullException(nameof(log));

            if (_compensationAddress == null)
                throw new InvalidCompensationAddressException(_compensationAddress);

            var result = new CompletedExecutionResult<TArguments>(this, Publisher, _activity, RoutingSlip, _compensationAddress);

            result.SetLog(log);

            return result;
        }

        public ExecutionResult Completed<TLog>(TLog log, ConfigureCompletedActivityOptionsCallback callback)
            where TLog : class
        {
            if (log == null)
                throw new ArgumentNullException(nameof(log));

            if (_compensationAddress == null)
                throw new InvalidCompensationAddressException(_compensationAddress);

            var result = new CompletedExecutionResult<TArguments>(this, Publisher, _activity, RoutingSlip, _compensationAddress);

            result.SetLog(log);

            callback(result);

            return result;
        }

        public ExecutionResult Completed<TLog>(object logValues)
            where TLog : class
        {
            if (logValues == null)
                throw new ArgumentNullException(nameof(logValues));

            if (_compensationAddress == null)
                throw new InvalidCompensationAddressException(_compensationAddress);

            var result = new CompletedExecutionResult<TArguments>(this, Publisher, _activity, RoutingSlip, _compensationAddress);

            result.SetLog(logValues);

            return result;
        }

        public ExecutionResult Completed<TLog>(object logValues, ConfigureCompletedActivityOptionsCallback callback)
            where TLog : class
        {
            if (logValues == null)
                throw new ArgumentNullException(nameof(logValues));

            if (_compensationAddress == null)
                throw new InvalidCompensationAddressException(_compensationAddress);

            var result = new CompletedExecutionResult<TArguments>(this, Publisher, _activity, RoutingSlip, _compensationAddress);

            result.SetLog(logValues);

            callback(result);

            return result;
        }

        public ExecutionResult CompletedWithVariables(IEnumerable<KeyValuePair<string, object>> variables)
        {
            if (variables == null)
                throw new ArgumentNullException(nameof(variables));

            var result = new CompletedExecutionResult<TArguments>(this, Publisher, _activity, RoutingSlip, _compensationAddress);

            result.SetVariables(variables);

            return result;
        }

        public ExecutionResult CompletedWithVariables(object variables)
        {
            if (variables == null)
                throw new ArgumentNullException(nameof(variables));

            var result = new CompletedExecutionResult<TArguments>(this, Publisher, _activity, RoutingSlip, _compensationAddress);

            result.SetVariables(variables);

            return result;
        }

        public ExecutionResult CompletedWithVariables<TLog>(TLog log, object variables)
            where TLog : class
        {
            if (log == null)
                throw new ArgumentNullException(nameof(log));

            if (variables == null)
                throw new ArgumentNullException(nameof(variables));

            if (_compensationAddress == null)
                throw new InvalidCompensationAddressException(_compensationAddress);

            var result = new CompletedExecutionResult<TArguments>(this, Publisher, _activity, RoutingSlip, _compensationAddress);

            result.SetLog(log);
            result.SetVariables(variables);

            return result;
        }

        public ExecutionResult CompletedWithVariables<TLog>(object logValues, object variables)
            where TLog : class
        {
            if (logValues == null)
                throw new ArgumentNullException(nameof(logValues));

            if (variables == null)
                throw new ArgumentNullException(nameof(variables));

            if (_compensationAddress == null)
                throw new InvalidCompensationAddressException(_compensationAddress);

            var result = new CompletedExecutionResult<TArguments>(this, Publisher, _activity, RoutingSlip, _compensationAddress);

            result.SetLog(logValues);
            result.SetVariables(variables);

            return result;
        }

        public ExecutionResult CompletedWithVariables<TLog>(TLog log, IEnumerable<KeyValuePair<string, object>> variables)
            where TLog : class
        {
            if (log == null)
                throw new ArgumentNullException(nameof(log));

            if (variables == null)
                throw new ArgumentNullException(nameof(variables));

            if (_compensationAddress == null)
                throw new InvalidCompensationAddressException(_compensationAddress);

            var result = new CompletedExecutionResult<TArguments>(this, Publisher, _activity, RoutingSlip, _compensationAddress);

            result.SetLog(log);
            result.SetVariables(variables);

            return result;
        }

        public ExecutionResult ReviseItinerary(Action<IItineraryBuilder> buildItinerary)
        {
            if (buildItinerary == null)
                throw new ArgumentNullException(nameof(buildItinerary));

            return new ReviseItineraryExecutionResult<TArguments>(this, Publisher, _activity, RoutingSlip, _compensationAddress, buildItinerary);
        }

        public ExecutionResult ReviseItinerary<TLog>(TLog log, Action<IItineraryBuilder> buildItinerary)
            where TLog : class
        {
            if (log == null)
                throw new ArgumentNullException(nameof(log));

            if (buildItinerary == null)
                throw new ArgumentNullException(nameof(buildItinerary));

            if (_compensationAddress == null)
                throw new InvalidCompensationAddressException(_compensationAddress);

            var result = new ReviseItineraryExecutionResult<TArguments>(this, Publisher, _activity, RoutingSlip, _compensationAddress, buildItinerary);

            result.SetLog(log);

            return result;
        }

        public ExecutionResult ReviseItinerary<TLog>(TLog log, object variables, Action<IItineraryBuilder> buildItinerary)
            where TLog : class
        {
            if (log == null)
                throw new ArgumentNullException(nameof(log));

            if (variables == null)
                throw new ArgumentNullException(nameof(variables));

            if (buildItinerary == null)
                throw new ArgumentNullException(nameof(buildItinerary));

            if (_compensationAddress == null)
                throw new InvalidCompensationAddressException(_compensationAddress);

            var result = new ReviseItineraryExecutionResult<TArguments>(this, Publisher, _activity, RoutingSlip, _compensationAddress, buildItinerary);

            result.SetLog(log);
            result.SetVariables(variables);

            return result;
        }

        public ExecutionResult ReviseItinerary<TLog>(TLog log, IEnumerable<KeyValuePair<string, object>> variables, Action<IItineraryBuilder> buildItinerary)
            where TLog : class
        {
            if (log == null)
                throw new ArgumentNullException(nameof(log));

            if (variables == null)
                throw new ArgumentNullException(nameof(variables));

            if (buildItinerary == null)
                throw new ArgumentNullException(nameof(buildItinerary));

            if (_compensationAddress == null)
                throw new InvalidCompensationAddressException(_compensationAddress);

            var result = new ReviseItineraryExecutionResult<TArguments>(this, Publisher, _activity, RoutingSlip, _compensationAddress, buildItinerary);

            result.SetLog(log);
            result.SetVariables(variables);

            return result;
        }

        public ExecutionResult Terminate()
        {
            return new TerminateExecutionResult<TArguments>(this, Publisher, _activity, RoutingSlip, _compensationAddress);
        }

        public ExecutionResult Terminate(object variables)
        {
            if (variables == null)
                throw new ArgumentNullException(nameof(variables));

            var result = new TerminateExecutionResult<TArguments>(this, Publisher, _activity, RoutingSlip, _compensationAddress);

            result.SetVariables(variables);

            return result;
        }

        public ExecutionResult Terminate(IEnumerable<KeyValuePair<string, object>> variables)
        {
            if (variables == null)
                throw new ArgumentNullException(nameof(variables));

            var result = new TerminateExecutionResult<TArguments>(this, Publisher, _activity, RoutingSlip, _compensationAddress);

            result.SetVariables(variables);

            return result;
        }

        public ExecutionResult Faulted()
        {
            return Faulted(new ActivityExecutionFaultedException());
        }

        public ExecutionResult Faulted(Exception exception)
        {
            if (exception == null)
                throw new ArgumentNullException(nameof(exception));

            return new FaultedExecutionResult<TArguments>(this, Publisher, _activity, RoutingSlip, exception);
        }

        public ExecutionResult Faulted(Exception exception, ConfigureFaultedActivityOptionsCallback callback)
        {
            if (exception == null)
                throw new ArgumentNullException(nameof(exception));

            var result = new FaultedExecutionResult<TArguments>(this, Publisher, _activity, RoutingSlip, exception);

            callback?.Invoke(result);

            return result;
        }

        public ExecutionResult FaultedWithVariables(Exception exception, object variables)
        {
            if (exception == null)
                throw new ArgumentNullException(nameof(exception));
            if (variables == null)
                throw new ArgumentNullException(nameof(variables));

            return Faulted(exception, x => x.SetVariables(variables));
        }

        public ExecutionResult FaultedWithVariables(Exception exception, IEnumerable<KeyValuePair<string, object>> variables)
        {
            if (exception == null)
                throw new ArgumentNullException(nameof(exception));
            if (variables == null)
                throw new ArgumentNullException(nameof(variables));

            return Faulted(exception, x => x.SetVariables(variables));
        }
    }
}
