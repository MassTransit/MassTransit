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
namespace MassTransit.AzureServiceBusTransport.Transport
{
    using System;
    using System.Threading.Tasks;
    using Context;
    using Contexts;
    using GreenPipes;
    using Logging;
    using MassTransit.Pipeline;
    using MassTransit.Pipeline.Observables;
    using MassTransit.Topology;
    using Microsoft.ServiceBus.Messaging;
    using Transports;
    using Util;


    /// <summary>
    /// Receives a <see cref="BrokeredMessage"/>.
    /// </summary>
    public class BrokeredMessageReceiver :
        IBrokeredMessageReceiver
    {
        readonly Uri _inputAddress;
        readonly ILog _log;
        readonly ReceiveObservable _receiveObservers;
        readonly IReceivePipe _receivePipe;
        readonly ReceiveEndpointContext _receiveTopology;

        public BrokeredMessageReceiver(Uri inputAddress, IReceivePipe receivePipe, ILog log, ReceiveEndpointContext receiveTopology)
        {
            _inputAddress = inputAddress;
            _receivePipe = receivePipe;
            _log = log;
            _receiveTopology = receiveTopology;
            _receiveObservers = new ReceiveObservable();
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("receiver");
            scope.Add("type", "brokeredMessage");
        }

        ConnectHandle IReceiveObserverConnector.ConnectReceiveObserver(IReceiveObserver observer)
        {
            return _receiveObservers.Connect(observer);
        }

        ConnectHandle IPublishObserverConnector.ConnectPublishObserver(IPublishObserver observer)
        {
            return _receiveTopology.PublishEndpointProvider.ConnectPublishObserver(observer);
        }

        ConnectHandle ISendObserverConnector.ConnectSendObserver(ISendObserver observer)
        {
            var sendHandle = _receiveTopology.ConnectSendObserver(observer);
            var publishHandle = _receiveTopology.ConnectSendObserver(observer);

            return new MultipleConnectHandle(sendHandle, publishHandle);
        }

        async Task IBrokeredMessageReceiver.Handle(BrokeredMessage message, Action<ReceiveContext> contextCallback)
        {
            var context = new ServiceBusReceiveContext(_inputAddress, message, _receiveObservers, _receiveTopology);
            contextCallback?.Invoke(context);

            try
            {
                await _receiveObservers.PreReceive(context).ConfigureAwait(false);

                if (message.LockedUntilUtc <= DateTime.UtcNow)
                    throw new MessageLockExpiredException(_inputAddress, $"The message lock expired: {message.MessageId}");

                if (message.ExpiresAtUtc < DateTime.UtcNow)
                    throw new MessageTimeToLiveExpiredException(_inputAddress, $"The message TTL expired: {message.MessageId}");

                await _receivePipe.Send(context).ConfigureAwait(false);

                await context.CompleteTask.ConfigureAwait(false);

                await message.CompleteAsync().ConfigureAwait(false);

                await _receiveObservers.PostReceive(context).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                try
                {
                    await message.AbandonAsync().ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    if (_log.IsWarnEnabled)
                        _log.Warn($"Abandon message faulted: {message.MessageId}", exception);
                }

                await _receiveObservers.ReceiveFault(context, ex).ConfigureAwait(false);
            }
            finally
            {
                context.Dispose();
            }
        }

        ConnectHandle IConsumeMessageObserverConnector.ConnectConsumeMessageObserver<T>(IConsumeMessageObserver<T> observer)
        {
            return _receivePipe.ConnectConsumeMessageObserver(observer);
        }

        ConnectHandle IConsumeObserverConnector.ConnectConsumeObserver(IConsumeObserver observer)
        {
            return _receivePipe.ConnectConsumeObserver(observer);
        }
    }
}