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
namespace MassTransit.Context
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using Pipeline;
    using Util;


    /// <summary>
    /// Handles the sending of a request to the endpoint specified
    /// </summary>
    /// <typeparam name="TRequest">The request type</typeparam>
    public class SendRequest<TRequest> :
        IPipe<SendContext<TRequest>>,
        Request<TRequest>
        where TRequest : class
    {
        readonly Action<IRequestConfigurator<TRequest>> _callback;
        readonly IRequestPipeConnector _connector;
        readonly Guid _requestId;
        readonly Uri _responseAddress;
        readonly TaskScheduler _taskScheduler;
        SendRequestConfigurator<TRequest> _requestConfigurator;

        public SendRequest(IRequestPipeConnector connector, Uri responseAddress, TaskScheduler taskScheduler, Action<IRequestConfigurator<TRequest>> callback)
        {
            _taskScheduler = taskScheduler;
            _callback = callback;
            _connector = connector;
            _responseAddress = responseAddress;
            _requestId = NewId.NextGuid();
        }

        void IProbeSite.Probe(ProbeContext context)
        {
        }

        Task IPipe<SendContext<TRequest>>.Send(SendContext<TRequest> context)
        {
            context.RequestId = _requestId;
            context.ResponseAddress = _responseAddress;

            if (_requestConfigurator == null)
                _requestConfigurator = new SendRequestConfigurator<TRequest>(_connector, context, _taskScheduler, _callback);
            else
            {
                var publishContext = new PublishRequestConfigurator<TRequest>(context, _callback, _requestConfigurator.Connections,
                    ((IRequestConfigurator<TRequest>)_requestConfigurator).Task);
            }

            return TaskUtil.Completed;
        }

        Task<TRequest> Request<TRequest>.Task => ((IRequestConfigurator<TRequest>)_requestConfigurator).Task;
    }
}