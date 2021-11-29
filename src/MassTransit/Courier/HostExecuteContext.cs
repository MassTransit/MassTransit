namespace MassTransit.Courier
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
            return new NextActivityExecutionResult<TArguments>(this, Publisher, _activity, RoutingSlip);
        }

        public ExecutionResult Completed<TLog>(TLog log)
            where TLog : class
        {
            if (log == null)
                throw new ArgumentNullException(nameof(log));

            if (_compensationAddress == null)
                throw new InvalidCompensationAddressException(_compensationAddress);

            return new NextActivityExecutionResult<TArguments, TLog>(this, Publisher, _activity, RoutingSlip, _compensationAddress, log);
        }

        public ExecutionResult Completed<TLog>(object logValues)
            where TLog : class
        {
            if (logValues == null)
                throw new ArgumentNullException(nameof(logValues));

            if (_compensationAddress == null)
                throw new InvalidCompensationAddressException(_compensationAddress);

            return new NextActivityExecutionResult<TArguments, TLog>(this, Publisher, _activity, RoutingSlip, _compensationAddress,
                RoutingSlipBuilder.GetObjectAsDictionary(logValues));
        }

        public ExecutionResult CompletedWithVariables(IEnumerable<KeyValuePair<string, object>> variables)
        {
            if (variables == null)
                throw new ArgumentNullException(nameof(variables));

            return new NextActivityWithVariablesExecutionResult<TArguments>(this, Publisher, _activity, RoutingSlip,
                variables.ToDictionary(x => x.Key, x => x.Value));
        }

        public ExecutionResult CompletedWithVariables(object variables)
        {
            if (variables == null)
                throw new ArgumentNullException(nameof(variables));

            return new NextActivityWithVariablesExecutionResult<TArguments>(this, Publisher, _activity, RoutingSlip,
                RoutingSlipBuilder.GetObjectAsDictionary(variables));
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

            return new NextActivityWithVariablesExecutionResult<TArguments, TLog>(this, Publisher, _activity, RoutingSlip, _compensationAddress,
                log,
                RoutingSlipBuilder.GetObjectAsDictionary(variables));
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

            return new NextActivityWithVariablesExecutionResult<TArguments, TLog>(this, Publisher, _activity, RoutingSlip, _compensationAddress,
                RoutingSlipBuilder.GetObjectAsDictionary(logValues), RoutingSlipBuilder.GetObjectAsDictionary(variables));
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

            return new NextActivityWithVariablesExecutionResult<TArguments, TLog>(this, Publisher, _activity, RoutingSlip, _compensationAddress,
                log,
                variables.ToDictionary(x => x.Key, x => x.Value));
        }

        public ExecutionResult ReviseItinerary(Action<IItineraryBuilder> buildItinerary)
        {
            if (buildItinerary == null)
                throw new ArgumentNullException(nameof(buildItinerary));

            return new ReviseItineraryExecutionResult<TArguments>(this, Publisher, _activity, RoutingSlip, buildItinerary);
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

            return new ReviseItineraryExecutionResult<TArguments, TLog>(this, Publisher, _activity, RoutingSlip, _compensationAddress, log,
                buildItinerary);
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

            return new ReviseItineraryWithVariablesExecutionResult<TArguments, TLog>(this, Publisher, _activity, RoutingSlip,
                _compensationAddress,
                log, RoutingSlipBuilder.GetObjectAsDictionary(variables), buildItinerary);
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

            return new ReviseItineraryWithVariablesExecutionResult<TArguments, TLog>(this, Publisher, _activity, RoutingSlip,
                _compensationAddress,
                log, variables.ToDictionary(x => x.Key, x => x.Value), buildItinerary);
        }

        public ExecutionResult Terminate()
        {
            return new TerminateExecutionResult<TArguments>(this, Publisher, _activity, RoutingSlip);
        }

        public ExecutionResult Terminate(object variables)
        {
            if (variables == null)
                throw new ArgumentNullException(nameof(variables));

            return new TerminateWithVariablesExecutionResult<TArguments>(this, Publisher, _activity, RoutingSlip,
                RoutingSlipBuilder.GetObjectAsDictionary(variables));
        }

        public ExecutionResult Terminate(IEnumerable<KeyValuePair<string, object>> variables)
        {
            if (variables == null)
                throw new ArgumentNullException(nameof(variables));

            return new TerminateWithVariablesExecutionResult<TArguments>(this, Publisher, _activity, RoutingSlip,
                variables.ToDictionary(x => x.Key, x => x.Value));
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

        public ExecutionResult FaultedWithVariables(Exception exception, object variables)
        {
            if (exception == null)
                throw new ArgumentNullException(nameof(exception));

            if (variables == null)
                throw new ArgumentNullException(nameof(variables));

            return new FaultedWithVariablesExecutionResult<TArguments>(this, Publisher, _activity, RoutingSlip, exception,
                RoutingSlipBuilder.GetObjectAsDictionary(variables));
        }

        public ExecutionResult FaultedWithVariables(Exception exception, IEnumerable<KeyValuePair<string, object>> variables)
        {
            if (exception == null)
                throw new ArgumentNullException(nameof(exception));

            if (variables == null)
                throw new ArgumentNullException(nameof(variables));

            return new FaultedWithVariablesExecutionResult<TArguments>(this, Publisher, _activity, RoutingSlip, exception,
                variables.ToDictionary(x => x.Key, x => x.Value));
        }
    }
}
