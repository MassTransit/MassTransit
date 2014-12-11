// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Contracts;
    using Events;
    using InternalMessages;
    using MassTransit.Events;
    using Pipeline;


    public class HostExecution<TArguments> :
        Execution<TArguments>
        where TArguments : class
    {
        readonly Activity _activity;
        readonly Guid _activityTrackingNumber;
        readonly TArguments _arguments;
        readonly Uri _compensationAddress;
        readonly ConsumeContext<RoutingSlip> _context;
        readonly HostInfo _host;
        readonly SanitizedRoutingSlip _routingSlip;
        readonly Stopwatch _timer;
        readonly DateTime _timestamp;

        public HostExecution(HostInfo host, Uri compensationAddress, ConsumeContext<RoutingSlip> context)
        {
            _host = host;
            _compensationAddress = compensationAddress;
            _context = context;

            _timer = Stopwatch.StartNew();
            NewId newId = NewId.Next();

            _activityTrackingNumber = newId.ToGuid();
            _timestamp = newId.Timestamp;

            _routingSlip = new SanitizedRoutingSlip(context);
            if (_routingSlip.Itinerary.Count == 0)
                throw new ArgumentException("The routingSlip must contain at least one activity");

            _activity = _routingSlip.Itinerary[0];
            _arguments = _routingSlip.GetActivityArguments<TArguments>();
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

        TArguments Execution<TArguments>.Arguments
        {
            get { return _arguments; }
        }

        Guid Execution<TArguments>.TrackingNumber
        {
            get { return _routingSlip.TrackingNumber; }
        }

        Guid Execution<TArguments>.ActivityTrackingNumber
        {
            get { return _activityTrackingNumber; }
        }

        public string ActivityName
        {
            get { return _activity.Name; }
        }

        ExecutionResult Execution<TArguments>.Completed()
        {
            return new NextActivityExecutionResult<TArguments>(this, _activity, _routingSlip);
        }

        ExecutionResult Execution<TArguments>.Completed<TLog>(TLog log)
        {
            if (log == null)
                throw new ArgumentNullException("log");

            if (_compensationAddress == null)
                throw new InvalidCompensationAddressException(_compensationAddress);

            return new NextActivityExecutionResult<TArguments, TLog>(this, _activity, _routingSlip, _compensationAddress, log);
        }

        ExecutionResult Execution<TArguments>.CompletedWithVariables(IEnumerable<KeyValuePair<string, object>> variables)
        {
            if (variables == null)
                throw new ArgumentNullException("variables");

            return new NextActivityWithVariablesExecutionResult<TArguments>(this, _activity, _routingSlip,
                variables.ToDictionary(x => x.Key, x => x.Value));
        }

        ExecutionResult Execution<TArguments>.CompletedWithVariables(object variables)
        {
            if (variables == null)
                throw new ArgumentNullException("variables");

            return new NextActivityWithVariablesExecutionResult<TArguments>(this, _activity, _routingSlip,
                RoutingSlipBuilder.GetObjectAsDictionary(variables));
        }

        ExecutionResult Execution<TArguments>.CompletedWithVariables<TLog>(TLog log, object variables)
        {
            if (log == null)
                throw new ArgumentNullException("log");
            if (variables == null)
                throw new ArgumentNullException("variables");

            if (_compensationAddress == null)
                throw new InvalidCompensationAddressException(_compensationAddress);

            return new NextActivityWithVariablesExecutionResult<TArguments, TLog>(this, _activity, _routingSlip, _compensationAddress, log,
                RoutingSlipBuilder.GetObjectAsDictionary(variables));
        }

        ExecutionResult Execution<TArguments>.CompletedWithVariables<TLog>(TLog log, IEnumerable<KeyValuePair<string, object>> variables)
        {
            if (log == null)
                throw new ArgumentNullException("log");
            if (variables == null)
                throw new ArgumentNullException("variables");

            if (_compensationAddress == null)
                throw new InvalidCompensationAddressException(_compensationAddress);

            return new NextActivityWithVariablesExecutionResult<TArguments, TLog>(this, _activity, _routingSlip, _compensationAddress, log,
                variables.ToDictionary(x => x.Key, x => x.Value));
        }

        public ExecutionResult ReviseItinerary(Action<ItineraryBuilder> buildItinerary)
        {
            if (buildItinerary == null)
                throw new ArgumentNullException("buildItinerary");

            return new ReviseItineraryExecutionResult<TArguments>(this, _activity, _routingSlip, buildItinerary);
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

            return new ReviseItineraryExecutionResult<TArguments, TLog>(this, _activity, _routingSlip, _compensationAddress, log,
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

            return new ReviseItineraryWithVariablesExecutionResult<TArguments, TLog>(this, _activity, _routingSlip, _compensationAddress,
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

            return new ReviseItineraryWithVariablesExecutionResult<TArguments, TLog>(this, _activity, _routingSlip, _compensationAddress,
                log, variables.ToDictionary(x => x.Key, x => x.Value), buildItinerary);
        }

        ExecutionResult Execution<TArguments>.Faulted()
        {
            return Faulted(new ActivityExecutionFaultedException());
        }

        ExecutionResult Execution<TArguments>.Faulted(Exception exception)
        {
            if (exception == null)
                throw new ArgumentNullException("exception");

            return Faulted(exception);
        }

        ExecutionResult Faulted(Exception exception)
        {
            return new FaultedExecutionResult<TArguments>(this, _activity, _routingSlip, new FaultExceptionInfo(exception));
        }

        public Task<ISendEndpoint> GetSendEndpoint(Uri address)
        {
            return _context.GetSendEndpoint(address);
        }
    }
}