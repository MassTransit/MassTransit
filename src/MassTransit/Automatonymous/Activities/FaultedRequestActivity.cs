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
namespace Automatonymous.Activities
{
    using System;
    using System.Threading.Tasks;

    public class FaultedRequestActivity<TInstance, TException, TRequest, TResponse> :
        RequestActivityImpl<TInstance, TRequest, TResponse>,
        Activity<TInstance>
        where TInstance : class, SagaStateMachineInstance
        where TException : Exception
        where TRequest : class
        where TResponse : class
    {
        readonly EventExceptionMessageFactory<TInstance, TException, TRequest> _messageFactory;
        readonly AsyncEventExceptionMessageFactory<TInstance, TException, TRequest> _asyncMessageFactory;
        readonly ServiceAddressExceptionProvider<TInstance, TException> _serviceAddressProvider;

        public FaultedRequestActivity(Request<TInstance, TRequest, TResponse> request,
            EventExceptionMessageFactory<TInstance, TException, TRequest> messageFactory)
            : base(request)
        {
            _messageFactory = messageFactory;
            _serviceAddressProvider = context => request.Settings.ServiceAddress;
        }

        public FaultedRequestActivity(Request<TInstance, TRequest, TResponse> request,
            ServiceAddressExceptionProvider<TInstance, TException> serviceAddressProvider,
            EventExceptionMessageFactory<TInstance, TException, TRequest> messageFactory)
            : base(request)
        {
            _messageFactory = messageFactory;
            _serviceAddressProvider = context => serviceAddressProvider(context) ?? request.Settings.ServiceAddress;
        }

        public FaultedRequestActivity(Request<TInstance, TRequest, TResponse> request,
            AsyncEventExceptionMessageFactory<TInstance, TException, TRequest> messageFactory)
            : base(request)
        {
            _asyncMessageFactory = messageFactory;
            _serviceAddressProvider = context => request.Settings.ServiceAddress;
        }

        public FaultedRequestActivity(Request<TInstance, TRequest, TResponse> request,
            ServiceAddressExceptionProvider<TInstance, TException> serviceAddressProvider,
            AsyncEventExceptionMessageFactory<TInstance, TException, TRequest> messageFactory)
            : base(request)
        {
            _asyncMessageFactory = messageFactory;
            _serviceAddressProvider = context => serviceAddressProvider(context) ?? request.Settings.ServiceAddress;
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        Task Activity<TInstance>.Execute(BehaviorContext<TInstance> context, Behavior<TInstance> next)
        {
            return next.Execute(context);
        }

        Task Activity<TInstance>.Execute<T>(BehaviorContext<TInstance, T> context, Behavior<TInstance, T> next)
        {
            return next.Execute(context);
        }

        async Task Activity<TInstance>.Faulted<T>(BehaviorExceptionContext<TInstance, T> context, Behavior<TInstance> next)
        {
            if (context.TryGetExceptionContext(out ConsumeExceptionEventContext<TInstance, TException> exceptionContext))
            {
                var message = _messageFactory?.Invoke(exceptionContext) ?? await _asyncMessageFactory(exceptionContext).ConfigureAwait(false);
                var serviceAddress = _serviceAddressProvider(exceptionContext);

                await SendRequest(context, exceptionContext, message, serviceAddress).ConfigureAwait(false);
            }

            await next.Faulted(context).ConfigureAwait(false);
        }

        async Task Activity<TInstance>.Faulted<T, TOtherException>(BehaviorExceptionContext<TInstance, T, TOtherException> context, Behavior<TInstance, T> next)
        {
            if (context.TryGetExceptionContext(out ConsumeExceptionEventContext<TInstance, TException> exceptionContext))
            {
                var message = _messageFactory?.Invoke(exceptionContext) ?? await _asyncMessageFactory(exceptionContext).ConfigureAwait(false);
                var serviceAddress = _serviceAddressProvider(exceptionContext);

                await SendRequest(context, exceptionContext, message, serviceAddress).ConfigureAwait(false);
            }

            await next.Faulted(context).ConfigureAwait(false);
        }
    }

    public class FaultedRequestActivity<TInstance, TData, TException, TRequest, TResponse> :
        RequestActivityImpl<TInstance, TRequest, TResponse>,
        Activity<TInstance, TData>
        where TInstance : class, SagaStateMachineInstance
        where TData : class
        where TException : Exception
        where TRequest : class
        where TResponse : class
    {
        readonly EventExceptionMessageFactory<TInstance, TData, TException, TRequest> _messageFactory;
        readonly AsyncEventExceptionMessageFactory<TInstance, TData, TException, TRequest> _asyncMessageFactory;
        readonly ServiceAddressExceptionProvider<TInstance, TData, TException> _serviceAddressProvider;

        public FaultedRequestActivity(Request<TInstance, TRequest, TResponse> request,
            EventExceptionMessageFactory<TInstance, TData, TException, TRequest> messageFactory)
            : base(request)
        {
            _messageFactory = messageFactory;
            _serviceAddressProvider = context => request.Settings.ServiceAddress;
        }

        public FaultedRequestActivity(Request<TInstance, TRequest, TResponse> request,
            ServiceAddressExceptionProvider<TInstance, TData, TException> serviceAddressProvider,
            EventExceptionMessageFactory<TInstance, TData, TException, TRequest> messageFactory)
            : base(request)
        {
            _messageFactory = messageFactory;
            _serviceAddressProvider = context => serviceAddressProvider(context) ?? request.Settings.ServiceAddress;
        }

        public FaultedRequestActivity(Request<TInstance, TRequest, TResponse> request,
            AsyncEventExceptionMessageFactory<TInstance, TData, TException, TRequest> messageFactory)
            : base(request)
        {
            _asyncMessageFactory = messageFactory;
            _serviceAddressProvider = context => request.Settings.ServiceAddress;
        }

        public FaultedRequestActivity(Request<TInstance, TRequest, TResponse> request,
            ServiceAddressExceptionProvider<TInstance, TData, TException> serviceAddressProvider,
            AsyncEventExceptionMessageFactory<TInstance, TData, TException, TRequest> messageFactory)
            : base(request)
        {
            _asyncMessageFactory = messageFactory;
            _serviceAddressProvider = context => serviceAddressProvider(context) ?? request.Settings.ServiceAddress;
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        Task Activity<TInstance, TData>.Execute(BehaviorContext<TInstance, TData> context, Behavior<TInstance, TData> next)
        {
            return next.Execute(context);
        }

        async Task Activity<TInstance, TData>.Faulted<T>(BehaviorExceptionContext<TInstance, TData, T> context, Behavior<TInstance, TData> next)
        {
            if (context.TryGetExceptionContext(out ConsumeExceptionEventContext<TInstance, TData, TException> exceptionContext))
            {
                var message = _messageFactory?.Invoke(exceptionContext) ?? await _asyncMessageFactory(exceptionContext).ConfigureAwait(false);
                var serviceAddress = _serviceAddressProvider(exceptionContext);

                await SendRequest(context, exceptionContext, message, serviceAddress).ConfigureAwait(false);
            }

            await next.Faulted(context).ConfigureAwait(false);
        }
    }
}
