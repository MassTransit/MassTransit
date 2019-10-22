namespace MassTransit.Courier.Contexts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Contracts;
    using Exceptions;
    using Results;


    public class HostExecuteContext<TArguments> :
        BaseCourierContext,
        ExecuteContext<TArguments>
        where TArguments : class
    {
        readonly Activity _activity;
        readonly TArguments _arguments;
        readonly Uri _compensationAddress;

        public HostExecuteContext(Uri compensationAddress, ConsumeContext<RoutingSlip> context)
            : base(context)
        {
            _compensationAddress = compensationAddress;

            if (RoutingSlip.Itinerary.Count == 0)
                throw new ArgumentException("The routingSlip must contain at least one activity");

            _activity = RoutingSlip.Itinerary[0];
            _arguments = RoutingSlip.GetActivityArguments<TArguments>();
        }

        public override string ActivityName => _activity.Name;
        TArguments ExecuteContext<TArguments>.Arguments => _arguments;

        public ExecuteActivityContext<TActivity, TArguments> CreateActivityContext<TActivity>(TActivity activity)
            where TActivity : class, IExecuteActivity<TArguments>
        {
            return new HostExecuteActivityContext<TActivity, TArguments>(activity, this);
        }

        public ExecutionResult Result { get; set; }

        ExecutionResult ExecuteContext.Completed()
        {
            return (new NextActivityExecutionResult<TArguments>(this, Publisher, _activity, RoutingSlip));
        }

        ExecutionResult ExecuteContext.Completed<TLog>(TLog log)
        {
            if (log == null)
                throw new ArgumentNullException(nameof(log));

            if (_compensationAddress == null)
                throw new InvalidCompensationAddressException(_compensationAddress);

            return new NextActivityExecutionResult<TArguments, TLog>(this, Publisher, _activity, RoutingSlip, _compensationAddress, log);
        }

        ExecutionResult ExecuteContext.Completed<TLog>(object logValues)
        {
            if (logValues == null)
                throw new ArgumentNullException(nameof(logValues));

            if (_compensationAddress == null)
                throw new InvalidCompensationAddressException(_compensationAddress);

            return (new NextActivityExecutionResult<TArguments, TLog>(this, Publisher, _activity, RoutingSlip, _compensationAddress,
                RoutingSlipBuilder.GetObjectAsDictionary(logValues)));
        }

        ExecutionResult ExecuteContext.CompletedWithVariables(IEnumerable<KeyValuePair<string, object>> variables)
        {
            if (variables == null)
                throw new ArgumentNullException(nameof(variables));

            return (new NextActivityWithVariablesExecutionResult<TArguments>(this, Publisher, _activity, RoutingSlip,
                variables.ToDictionary(x => x.Key, x => x.Value)));
        }

        ExecutionResult ExecuteContext.CompletedWithVariables(object variables)
        {
            if (variables == null)
                throw new ArgumentNullException(nameof(variables));

            return (new NextActivityWithVariablesExecutionResult<TArguments>(this, Publisher, _activity, RoutingSlip,
                RoutingSlipBuilder.GetObjectAsDictionary(variables)));
        }

        ExecutionResult ExecuteContext.CompletedWithVariables<TLog>(TLog log, object variables)
        {
            if (log == null)
                throw new ArgumentNullException(nameof(log));

            if (variables == null)
                throw new ArgumentNullException(nameof(variables));

            if (_compensationAddress == null)
                throw new InvalidCompensationAddressException(_compensationAddress);

            return (new NextActivityWithVariablesExecutionResult<TArguments, TLog>(this, Publisher, _activity, RoutingSlip, _compensationAddress,
                log,
                RoutingSlipBuilder.GetObjectAsDictionary(variables)));
        }

        ExecutionResult ExecuteContext.CompletedWithVariables<TLog>(object logValues, object variables)
        {
            if (logValues == null)
                throw new ArgumentNullException(nameof(logValues));

            if (variables == null)
                throw new ArgumentNullException(nameof(variables));

            if (_compensationAddress == null)
                throw new InvalidCompensationAddressException(_compensationAddress);

            return (new NextActivityWithVariablesExecutionResult<TArguments, TLog>(this, Publisher, _activity, RoutingSlip, _compensationAddress,
                RoutingSlipBuilder.GetObjectAsDictionary(logValues), RoutingSlipBuilder.GetObjectAsDictionary(variables)));
        }

        ExecutionResult ExecuteContext.CompletedWithVariables<TLog>(TLog log, IEnumerable<KeyValuePair<string, object>> variables)
        {
            if (log == null)
                throw new ArgumentNullException(nameof(log));

            if (variables == null)
                throw new ArgumentNullException(nameof(variables));

            if (_compensationAddress == null)
                throw new InvalidCompensationAddressException(_compensationAddress);

            return (new NextActivityWithVariablesExecutionResult<TArguments, TLog>(this, Publisher, _activity, RoutingSlip, _compensationAddress,
                log,
                variables.ToDictionary(x => x.Key, x => x.Value)));
        }

        ExecutionResult ExecuteContext.ReviseItinerary(Action<ItineraryBuilder> buildItinerary)
        {
            if (buildItinerary == null)
                throw new ArgumentNullException(nameof(buildItinerary));

            return (new ReviseItineraryExecutionResult<TArguments>(this, Publisher, _activity, RoutingSlip, buildItinerary));
        }

        ExecutionResult ExecuteContext.ReviseItinerary<TLog>(TLog log, Action<ItineraryBuilder> buildItinerary)
        {
            if (log == null)
                throw new ArgumentNullException(nameof(log));

            if (buildItinerary == null)
                throw new ArgumentNullException(nameof(buildItinerary));

            if (_compensationAddress == null)
                throw new InvalidCompensationAddressException(_compensationAddress);

            return (new ReviseItineraryExecutionResult<TArguments, TLog>(this, Publisher, _activity, RoutingSlip, _compensationAddress, log,
                buildItinerary));
        }

        ExecutionResult ExecuteContext.ReviseItinerary<TLog>(TLog log, object variables, Action<ItineraryBuilder> buildItinerary)
        {
            if (log == null)
                throw new ArgumentNullException(nameof(log));

            if (variables == null)
                throw new ArgumentNullException(nameof(variables));

            if (buildItinerary == null)
                throw new ArgumentNullException(nameof(buildItinerary));

            if (_compensationAddress == null)
                throw new InvalidCompensationAddressException(_compensationAddress);

            return (new ReviseItineraryWithVariablesExecutionResult<TArguments, TLog>(this, Publisher, _activity, RoutingSlip,
                _compensationAddress,
                log, RoutingSlipBuilder.GetObjectAsDictionary(variables), buildItinerary));
        }

        ExecutionResult ExecuteContext.ReviseItinerary<TLog>(TLog log, IEnumerable<KeyValuePair<string, object>> variables,
            Action<ItineraryBuilder> buildItinerary)
        {
            if (log == null)
                throw new ArgumentNullException(nameof(log));

            if (variables == null)
                throw new ArgumentNullException(nameof(variables));

            if (buildItinerary == null)
                throw new ArgumentNullException(nameof(buildItinerary));

            if (_compensationAddress == null)
                throw new InvalidCompensationAddressException(_compensationAddress);

            return (new ReviseItineraryWithVariablesExecutionResult<TArguments, TLog>(this, Publisher, _activity, RoutingSlip,
                _compensationAddress,
                log, variables.ToDictionary(x => x.Key, x => x.Value), buildItinerary));
        }

        ExecutionResult ExecuteContext.Terminate()
        {
            return (new TerminateExecutionResult<TArguments>(this, Publisher, _activity, RoutingSlip));
        }

        ExecutionResult ExecuteContext.Terminate(object variables)
        {
            if (variables == null)
                throw new ArgumentNullException(nameof(variables));

            return (new TerminateWithVariablesExecutionResult<TArguments>(this, Publisher, _activity, RoutingSlip,
                RoutingSlipBuilder.GetObjectAsDictionary(variables)));
        }

        ExecutionResult ExecuteContext.Terminate(IEnumerable<KeyValuePair<string, object>> variables)
        {
            if (variables == null)
                throw new ArgumentNullException(nameof(variables));

            return (new TerminateWithVariablesExecutionResult<TArguments>(this, Publisher, _activity, RoutingSlip,
                variables.ToDictionary(x => x.Key, x => x.Value)));
        }

        ExecutionResult ExecuteContext.Faulted()
        {
            return Faulted(new ActivityExecutionFaultedException());
        }

        public ExecutionResult Faulted(Exception exception)
        {
            if (exception == null)
                throw new ArgumentNullException(nameof(exception));

            return new FaultedExecutionResult<TArguments>(this, Publisher, _activity, RoutingSlip, exception);
        }
    }
}
