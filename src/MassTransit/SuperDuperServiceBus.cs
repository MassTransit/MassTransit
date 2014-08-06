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
namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Pipeline;
    using Transports;


    public class SuperDuperServiceBus :
        IBusControl
    {
        readonly IInboundPipe _inboundPipe;
        readonly IList<IReceiveEndpoint> _receiveEndpoints;
        readonly List<Task> _runningTasks;
        CancellationTokenSource _tokenSource;

        public SuperDuperServiceBus(IInboundPipe inboundPipe, IEnumerable<IReceiveEndpoint> receiveEndpoints)
        {
            _inboundPipe = inboundPipe;
            _receiveEndpoints = receiveEndpoints.ToList();
            _runningTasks = new List<Task>();
        }

        public void Dispose()
        {
            Stop().Wait();
        }

        Task IPublisher.Publish<T>(T message)
        {
            throw new NotImplementedException();
        }

        Task IPublisher.Publish<T>(T message, IPipe<PublishContext<T>> publishPipe)
        {
            throw new NotImplementedException();
        }

        Task IPublisher.Publish(object message)
        {
            throw new NotImplementedException();
        }

        Task IPublisher.Publish(object message, Type messageType)
        {
            throw new NotImplementedException();
        }

        Task IPublisher.Publish(object message, Action<PublishContext> contextCallback)
        {
            throw new NotImplementedException();
        }

        Task IPublisher.Publish(object message, Type messageType, Action<PublishContext> contextCallback)
        {
            throw new NotImplementedException();
        }

        Task IPublisher.Publish<T>(object values)
        {
            throw new NotImplementedException();
        }

        Task IPublisher.Publish<T>(object values, Action<PublishContext<T>> contextCallback)
        {
            throw new NotImplementedException();
        }

        IInboundPipe IBus.InboundPipe
        {
            get { return _inboundPipe; }
        }

        ISendEndpoint IBus.GetSendEndpoint(Uri address)
        {
            throw new NotImplementedException();
        }

        public async Task Start(CancellationToken cancellationToken)
        {
            _tokenSource = new CancellationTokenSource();
            using (cancellationToken.Register(_tokenSource.Cancel))
            {
                var startedTasks = new List<Task>();

                Exception exception = null;
                try
                {
                    startedTasks.AddRange(_receiveEndpoints.Select(receiveEndpoint => receiveEndpoint.Start(_tokenSource.Token)));

                    _runningTasks.AddRange(startedTasks);

                    startedTasks.Clear();
                }
                catch (Exception ex)
                {
                    exception = ex;
                }

                if (exception != null)
                {
                    _tokenSource.Cancel();

                    await Task.WhenAll(startedTasks);

                    throw new MassTransitException("The service bus could not be started", exception);
                }
            }
        }

        public async Task Stop()
        {
            _tokenSource.Cancel();

            await Task.WhenAll(_runningTasks);

            _tokenSource = null;
        }
    }
}