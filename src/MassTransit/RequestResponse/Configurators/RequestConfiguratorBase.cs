// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.RequestResponse.Configurators
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Magnum.Caching;
    using Magnum.Extensions;

    public abstract class RequestConfiguratorBase<TRequest>
        where TRequest : class
    {
        readonly IList<Action<ISendContext<TRequest>>> _contextActions;
        readonly Cache<Type, ResponseHandler> _handlers;
        readonly TRequest _message;
        readonly string _requestId;
        protected SynchronizationContext RequestSynchronizationContext;
        protected TimeSpan Timeout;
        protected TimeoutHandler<TRequest> TimeoutHandler;

        protected RequestConfiguratorBase(TRequest message)
        {
            _message = message;
            _requestId = NewId.NextGuid().ToString();
            Timeout = TimeSpan.FromMilliseconds(-1);

            _contextActions = new List<Action<ISendContext<TRequest>>>();
            _handlers = new DictionaryCache<Type, ResponseHandler>();
        }

        public TRequest Request
        {
            get { return _message; }
        }

        public string RequestId
        {
            get { return _requestId; }
        }

        protected IEnumerable<ResponseHandler> Handlers
        {
            get { return _handlers; }
        }

        public void SetTimeout(TimeSpan timeout)
        {
            Timeout = timeout;
        }

        public void HandleTimeout(TimeSpan timeout, Action timeoutCallback)
        {
            Timeout = timeout;
            TimeoutHandler = new TimeoutHandler<TRequest>(RequestSynchronizationContext, _ => timeoutCallback());
        }

        public void HandleTimeout(TimeSpan timeout, Action<TRequest> timeoutCallback)
        {
            Timeout = timeout;
            TimeoutHandler = new TimeoutHandler<TRequest>(RequestSynchronizationContext, timeoutCallback);
        }

        public void SetRequestExpiration(TimeSpan expiration)
        {
            _contextActions.Add(x => x.ExpiresIn(expiration));
        }

        public void ApplyContext(IPublishContext<TRequest> context, Uri responseAddress)
        {
            context.SetRequestId(_requestId);
            context.SendResponseTo(responseAddress);
            context.SendFaultTo(responseAddress);

            _contextActions.Each(x => x(context));
        }

        public void ApplyContext(ISendContext<TRequest> context, Uri responseAddress)
        {
            context.SetRequestId(_requestId);
            context.SetSourceAddress(responseAddress);
            context.SendResponseTo(responseAddress);
            context.SendFaultTo(responseAddress);

            _contextActions.Each(x => x(context));
        }


        protected T AddHandler<T>(Type responseType, Func<T> responseHandlerFactory)
            where T : class, ResponseHandler
        {
            if (_handlers.Has(responseType))
                throw new ArgumentException("A response handler for {0} has already been declared."
                    .FormatWith(responseType.ToShortTypeName()));

            T responseHandler = responseHandlerFactory();

            _handlers.Add(responseType, responseHandler);

            return responseHandler;
        }

        public void UseCurrentSynchronizationContext()
        {
            RequestSynchronizationContext = SynchronizationContext.Current;
        }

        public void SetSynchronizationContext(SynchronizationContext synchronizationContext)
        {
            RequestSynchronizationContext = synchronizationContext;
        }
    }
}