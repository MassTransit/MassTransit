// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.Courier.Hosts
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Contracts;
    using Events;
    using Exceptions;
    using MassTransit.Pipeline;
    using Results;


    public class HostExecuteContext<TArguments> :
        ExecuteContext<TArguments>
        where TArguments : class
    {
        readonly Activity _activity;
        readonly TArguments _arguments;
        readonly Uri _compensationAddress;
        readonly ConsumeContext<RoutingSlip> _context;
        readonly Guid _executionId;

        readonly HostInfo _host;
        readonly IRoutingSlipEventPublisher _publisher;
        readonly SanitizedRoutingSlip _routingSlip;
        readonly Stopwatch _timer;
        readonly DateTime _timestamp;

        public HostExecuteContext(HostInfo host, Uri compensationAddress, ConsumeContext<RoutingSlip> context)
        {
            _host = host;
            _compensationAddress = compensationAddress;
            _context = context;

            _timer = Stopwatch.StartNew();
            NewId newId = NewId.Next();

            _executionId = newId.ToGuid();
            _timestamp = newId.Timestamp;

            _routingSlip = new SanitizedRoutingSlip(context);
            if (_routingSlip.Itinerary.Count == 0)
                throw new ArgumentException("The routingSlip must contain at least one activity");

            _activity = _routingSlip.Itinerary[0];
            _arguments = _routingSlip.GetActivityArguments<TArguments>();

            _publisher = new RoutingSlipEventPublisher(this, _routingSlip);
        }

        CancellationToken PipeContext.CancellationToken
        {
            get { return _context.CancellationToken; }
        }

        bool PipeContext.HasPayloadType(Type contextType)
        {
            return _context.HasPayloadType(contextType);
        }

        bool PipeContext.TryGetPayload<TPayload>(out TPayload payload)
        {
            return _context.TryGetPayload(out payload);
        }

        TPayload PipeContext.GetOrAddPayload<TPayload>(PayloadFactory<TPayload> payloadFactory)
        {
            return _context.GetOrAddPayload(payloadFactory);
        }

        Task IPublishEndpoint.Publish<T>(T message, CancellationToken cancellationToken)
        {
            return _context.Publish(message, cancellationToken);
        }

        Task IPublishEndpoint.Publish<T>(T message, IPipe<PublishContext<T>> publishPipe,
            CancellationToken cancellationToken)
        {
            return _context.Publish(message, publishPipe, cancellationToken);
        }

        Task IPublishEndpoint.Publish<T>(T message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            return _context.Publish(message, publishPipe, cancellationToken);
        }

        Task IPublishEndpoint.Publish(object message, CancellationToken cancellationToken)
        {
            return _context.Publish(message, cancellationToken);
        }

        Task IPublishEndpoint.Publish(object message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            return _context.Publish(message, publishPipe, cancellationToken);
        }

        Task IPublishEndpoint.Publish(object message, Type messageType, CancellationToken cancellationToken)
        {
            return _context.Publish(message, messageType, cancellationToken);
        }

        Task IPublishEndpoint.Publish(object message, Type messageType, IPipe<PublishContext> publishPipe,
            CancellationToken cancellationToken)
        {
            return _context.Publish(message, messageType, publishPipe, cancellationToken);
        }

        Task IPublishEndpoint.Publish<T>(object values, CancellationToken cancellationToken)
        {
            return _context.Publish<T>(values, cancellationToken);
        }

        Task IPublishEndpoint.Publish<T>(object values, IPipe<PublishContext<T>> publishPipe,
            CancellationToken cancellationToken)
        {
            return _context.Publish(values, publishPipe, cancellationToken);
        }

        Task IPublishEndpoint.Publish<T>(object values, IPipe<PublishContext> publishPipe,
            CancellationToken cancellationToken)
        {
            return _context.Publish<T>(values, publishPipe, cancellationToken);
        }

        public HostInfo Host
        {
            get { return _host; }
        }

        public DateTime Timestamp
        {
            get { return _timestamp; }
        }

        public TimeSpan Elapsed
        {
            get { return _timer.Elapsed; }
        }

        public ConsumeContext ConsumeContext
        {
            get { return _context; }
        }

        TArguments ExecuteContext<TArguments>.Arguments
        {
            get { return _arguments; }
        }

        Guid ExecuteContext.TrackingNumber
        {
            get { return _routingSlip.TrackingNumber; }
        }

        Guid ExecuteContext.ExecutionId
        {
            get { return _executionId; }
        }

        public string ActivityName
        {
            get { return _activity.Name; }
        }

        ExecutionResult ExecuteContext.Completed()
        {
            return new NextActivityExecutionResult<TArguments>(this, _publisher, _activity, _routingSlip);
        }

        ExecutionResult ExecuteContext.Completed<TLog>(TLog log)
        {
            if (log == null)
                throw new ArgumentNullException("log");

            if (_compensationAddress == null)
                throw new InvalidCompensationAddressException(_compensationAddress);

            return new NextActivityExecutionResult<TArguments, TLog>(this, _publisher, _activity, _routingSlip, _compensationAddress, log);
        }

        ExecutionResult ExecuteContext.CompletedWithVariables(IEnumerable<KeyValuePair<string, object>> variables)
        {
            if (variables == null)
                throw new ArgumentNullException("variables");

            return new NextActivityWithVariablesExecutionResult<TArguments>(this, _publisher, _activity, _routingSlip,
                variables.ToDictionary(x => x.Key, x => x.Value));
        }

        ExecutionResult ExecuteContext.CompletedWithVariables(object variables)
        {
            if (variables == null)
                throw new ArgumentNullException("variables");

            return new NextActivityWithVariablesExecutionResult<TArguments>(this, _publisher, _activity, _routingSlip,
                RoutingSlipBuilder.GetObjectAsDictionary(variables));
        }

        ExecutionResult ExecuteContext.CompletedWithVariables<TLog>(TLog log, object variables)
        {
            if (log == null)
                throw new ArgumentNullException("log");
            if (variables == null)
                throw new ArgumentNullException("variables");

            if (_compensationAddress == null)
                throw new InvalidCompensationAddressException(_compensationAddress);

            return new NextActivityWithVariablesExecutionResult<TArguments, TLog>(this, _publisher, _activity, _routingSlip, _compensationAddress, log,
                RoutingSlipBuilder.GetObjectAsDictionary(variables));
        }

        ExecutionResult ExecuteContext.CompletedWithVariables<TLog>(TLog log, IEnumerable<KeyValuePair<string, object>> variables)
        {
            if (log == null)
                throw new ArgumentNullException("log");
            if (variables == null)
                throw new ArgumentNullException("variables");

            if (_compensationAddress == null)
                throw new InvalidCompensationAddressException(_compensationAddress);

            return new NextActivityWithVariablesExecutionResult<TArguments, TLog>(this, _publisher, _activity, _routingSlip, _compensationAddress, log,
                variables.ToDictionary(x => x.Key, x => x.Value));
        }

        public ExecutionResult ReviseItinerary(Action<ItineraryBuilder> buildItinerary)
        {
            if (buildItinerary == null)
                throw new ArgumentNullException("buildItinerary");

            return new ReviseItineraryExecutionResult<TArguments>(this, _publisher, _activity, _routingSlip, buildItinerary);
        }

        public ExecutionResult ReviseItinerary<TLog>(TLog log, Action<ItineraryBuilder> buildItinerary)
            where TLog : class
        {
            if (log == null)
                throw new ArgumentNullException("log");
            if (buildItinerary == null)
                throw new ArgumentNullException("buildItinerary");

            if (_compensationAddress == null)
                throw new InvalidCompensationAddressException(_compensationAddress);

            return new ReviseItineraryExecutionResult<TArguments, TLog>(this, _publisher, _activity, _routingSlip, _compensationAddress, log,
                buildItinerary);
        }

        public ExecutionResult ReviseItinerary<TLog>(TLog log, object variables, Action<ItineraryBuilder> buildItinerary)
            where TLog : class
        {
            if (log == null)
                throw new ArgumentNullException("log");
            if (variables == null)
                throw new ArgumentNullException("variables");
            if (buildItinerary == null)
                throw new ArgumentNullException("buildItinerary");

            if (_compensationAddress == null)
                throw new InvalidCompensationAddressException(_compensationAddress);

            return new ReviseItineraryWithVariablesExecutionResult<TArguments, TLog>(this, _publisher, _activity, _routingSlip, _compensationAddress,
                log, RoutingSlipBuilder.GetObjectAsDictionary(variables), buildItinerary);
        }

        public ExecutionResult ReviseItinerary<TLog>(TLog log, IEnumerable<KeyValuePair<string, object>> variables,
            Action<ItineraryBuilder> buildItinerary)
            where TLog : class
        {
            if (log == null)
                throw new ArgumentNullException("log");
            if (variables == null)
                throw new ArgumentNullException("variables");
            if (buildItinerary == null)
                throw new ArgumentNullException("buildItinerary");

            if (_compensationAddress == null)
                throw new InvalidCompensationAddressException(_compensationAddress);

            return new ReviseItineraryWithVariablesExecutionResult<TArguments, TLog>(this, _publisher, _activity, _routingSlip, _compensationAddress,
                log, variables.ToDictionary(x => x.Key, x => x.Value), buildItinerary);
        }

        public ExecutionResult Terminate()
        {
            return new TerminateExecutionResult<TArguments>(this, _publisher, _activity, _routingSlip);
        }

        public ExecutionResult Terminate(object variables)
        {
            if (variables == null)
                throw new ArgumentNullException("variables");

            return new TerminateWithVariablesExecutionResult<TArguments>(this, _publisher, _activity, _routingSlip,
                RoutingSlipBuilder.GetObjectAsDictionary(variables));
        }

        public ExecutionResult Terminate(IEnumerable<KeyValuePair<string, object>> variables)
        {
            if (variables == null)
                throw new ArgumentNullException("variables");

            return new TerminateWithVariablesExecutionResult<TArguments>(this, _publisher, _activity, _routingSlip,
                variables.ToDictionary(x => x.Key, x => x.Value));
        }

        ExecutionResult ExecuteContext.Faulted()
        {
            return Faulted(new ActivityExecutionFaultedException());
        }

        ExecutionResult ExecuteContext.Faulted(Exception exception)
        {
            if (exception == null)
                throw new ArgumentNullException("exception");

            return Faulted(exception);
        }

        public Task<ISendEndpoint> GetSendEndpoint(Uri address)
        {
            return _context.GetSendEndpoint(address);
        }

        ExecutionResult Faulted(Exception exception)
        {
            return new FaultedExecutionResult<TArguments>(this, _publisher, _activity, _routingSlip, new FaultExceptionInfo(exception));
        }
    }
}