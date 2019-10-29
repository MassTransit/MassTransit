// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Clients
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using ConsumeConfigurators;
    using GreenPipes;
    using GreenPipes.Util;
    using Pipeline;


    /// <summary>
    /// Connects a handler to the inbound pipe of the receive endpoint
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    public class ResponseHandlerConfigurator<TResponse> :
        IHandlerConfigurator<TResponse>
        where TResponse : class
    {
        readonly TaskCompletionSource<ConsumeContext<TResponse>> _completed;
        readonly MessageHandler<TResponse> _handler;
        readonly Task _requestTask;
        readonly IList<IPipeSpecification<ConsumeContext<TResponse>>> _specifications;
        readonly TaskScheduler _taskScheduler;

        public ResponseHandlerConfigurator(TaskScheduler taskScheduler, MessageHandler<TResponse> handler, Task requestTask)
        {
            _taskScheduler = taskScheduler;
            _handler = handler;
            _requestTask = requestTask;

            _specifications = new List<IPipeSpecification<ConsumeContext<TResponse>>>();
            _completed = Util.TaskUtil.GetTask<ConsumeContext<TResponse>>();
        }

        public void AddPipeSpecification(IPipeSpecification<ConsumeContext<TResponse>> specification)
        {
            _specifications.Add(specification);
        }

        public ConnectHandle ConnectHandlerConfigurationObserver(IHandlerConfigurationObserver observer)
        {
            return new EmptyConnectHandle();
        }

        public HandlerConnectHandle<TResponse> Connect(IRequestPipeConnector connector, Guid requestId)
        {
            MessageHandler<TResponse> messageHandler = _handler != null ? (MessageHandler<TResponse>)AsyncMessageHandler : MessageHandler;

            var connectHandle = connector.ConnectRequestHandler(requestId, messageHandler, _specifications.ToArray());

            return new ResponseHandlerConnectHandle<TResponse>(connectHandle, _completed, _requestTask);
        }

        async Task AsyncMessageHandler(ConsumeContext<TResponse> context)
        {
            try
            {
                await Task.Factory.StartNew(() => _handler(context), context.CancellationToken, TaskCreationOptions.None, _taskScheduler)
                    .Unwrap()
                    .ConfigureAwait(false);

                _completed.TrySetResult(context);
            }
            catch (Exception ex)
            {
                _completed.TrySetException(ex);
            }
        }

        Task MessageHandler(ConsumeContext<TResponse> context)
        {
            try
            {
                _completed.TrySetResult(context);
            }
            catch (Exception ex)
            {
                _completed.TrySetException(ex);
            }

            return Util.TaskUtil.Completed;
        }
    }
}
