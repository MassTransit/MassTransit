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
namespace MassTransit.Context
{
    using System;
    using System.Threading.Tasks;
    using Pipeline;


    /// <summary>
    /// Handles the sending of a request to the endpoint specified
    /// </summary>
    /// <typeparam name="TRequest">The request type</typeparam>
    public class SendRequest<TRequest> :
        IPipe<SendContext<TRequest>>,
        Request<TRequest>
        where TRequest : class
    {
        readonly IBus _bus;
        readonly Action<RequestContext<TRequest>> _callback;
        readonly TaskScheduler _taskScheduler;
        SendRequestContext<TRequest> _requestContext;

        public SendRequest(IBus bus, TaskScheduler taskScheduler, Action<RequestContext<TRequest>> callback)
        {
            _taskScheduler = taskScheduler;
            _callback = callback;
            _bus = bus;
        }

        async Task IProbeSite.Probe(ProbeContext context)
        {
        }

        public async Task Send(SendContext<TRequest> context)
        {
            context.RequestId = NewId.NextGuid();
            context.ResponseAddress = _bus.Address;

            _requestContext = new SendRequestContext<TRequest>(_bus, context, _taskScheduler, _callback);
        }

        public Task Task
        {
            get { return ((RequestContext)_requestContext).Task; }
        }
    }
}