// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using Util;


    public class HostExecuteActivityContext<TActivity, TArguments> :
        ExecuteActivityContext<TActivity, TArguments>
        where TArguments : class
        where TActivity : class, ExecuteActivity<TArguments>
    {
        readonly TActivity _activity;
        readonly ConsumeContext _consumeContext;
        readonly ExecuteContext<TArguments> _context;

        public HostExecuteActivityContext(TActivity activity, ExecuteContext<TArguments> context)
        {
            _activity = activity;
            _context = context;
            _consumeContext = _context.ConsumeContext;
        }

        Guid ExecuteContext.TrackingNumber => _context.TrackingNumber;
        Guid ExecuteContext.ExecutionId => _context.ExecutionId;
        HostInfo ExecuteContext.Host => _context.Host;
        DateTime ExecuteContext.Timestamp => _context.Timestamp;
        TimeSpan ExecuteContext.Elapsed => _context.Elapsed;
        ConsumeContext ExecuteContext.ConsumeContext => _context.ConsumeContext;
        string ExecuteContext.ActivityName => _context.ActivityName;

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

        ExecutionResult ExecuteContext.CompletedWithVariables<TLog>(TLog log, object variables)
        {
            return _context.CompletedWithVariables(log, variables);
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

        CancellationToken PipeContext.CancellationToken => _context.CancellationToken;

        ExecuteActivityContext<T, TArguments> ExecuteActivityContext<TArguments>.PopContext<T>()
        {
            var context = this as ExecuteActivityContext<T, TArguments>;
            if (context == null)
                throw new ContextException(
                    $"The ExecuteActivityContext<{TypeMetadataCache<TArguments>.ShortName}> could not be cast to {TypeMetadataCache<T>.ShortName}");

            return context;
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

        Task IPublishEndpoint.Publish<T>(T message, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken)
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

        Task IPublishEndpoint.Publish(object message, Type messageType, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            return _context.Publish(message, messageType, publishPipe, cancellationToken);
        }

        Task IPublishEndpoint.Publish<T>(object values, CancellationToken cancellationToken)
        {
            return _context.Publish<T>(values, cancellationToken);
        }

        Task IPublishEndpoint.Publish<T>(object values, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken)
        {
            return _context.Publish(values, publishPipe, cancellationToken);
        }

        Task IPublishEndpoint.Publish<T>(object values, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            return _context.Publish<T>(values, publishPipe, cancellationToken);
        }

        Task<ISendEndpoint> ISendEndpointProvider.GetSendEndpoint(Uri address)
        {
            return _consumeContext.GetSendEndpoint(address);
        }

        TActivity ExecuteActivityContext<TActivity, TArguments>.Activity => _activity;
        TArguments ExecuteContext<TArguments>.Arguments => _context.Arguments;

        ConnectHandle IPublishObserverConnector.ConnectPublishObserver(IPublishObserver observer)
        {
            return _consumeContext.ConnectPublishObserver(observer);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _consumeContext.ConnectSendObserver(observer);
        }
    }
}