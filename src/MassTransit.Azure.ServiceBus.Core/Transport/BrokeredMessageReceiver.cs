// Copyright 2007-2019 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Azure.ServiceBus.Core.Transport
{
    using System;
    using System.Threading.Tasks;
    using Context;
    using Contexts;
    using GreenPipes;
    using Logging;
    using MassTransit.Pipeline;
    using Microsoft.Azure.ServiceBus;
    using Transports;


    /// <summary>
    /// Receives a <see cref="Message"/>.
    /// </summary>
    public class BrokeredMessageReceiver :
        IBrokeredMessageReceiver
    {
        readonly Uri _inputAddress;
        readonly ILog _log;
        readonly ReceiveEndpointContext _receiveEndpointContext;

        public BrokeredMessageReceiver(Uri inputAddress, ILog log, ReceiveEndpointContext receiveEndpointContext)
        {
            _inputAddress = inputAddress;
            _log = log;
            _receiveEndpointContext = receiveEndpointContext;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("receiver");
            scope.Add("type", "brokeredMessage");
        }

        ConnectHandle IReceiveObserverConnector.ConnectReceiveObserver(IReceiveObserver observer)
        {
            return _receiveEndpointContext.ConnectReceiveObserver(observer);
        }

        ConnectHandle IPublishObserverConnector.ConnectPublishObserver(IPublishObserver observer)
        {
            return _receiveEndpointContext.ConnectPublishObserver(observer);
        }

        ConnectHandle ISendObserverConnector.ConnectSendObserver(ISendObserver observer)
        {
            return _receiveEndpointContext.ConnectSendObserver(observer);
        }

        async Task IBrokeredMessageReceiver.Handle(Message message, Action<ReceiveContext> contextCallback)
        {
            var context = new ServiceBusReceiveContext(_inputAddress, message, _receiveEndpointContext);
            contextCallback?.Invoke(context);

            context.TryGetPayload<Microsoft.Azure.ServiceBus.Core.IMessageReceiver>(out var messageReceiver);

            try
            {
                await _receiveEndpointContext.ReceiveObservers.PreReceive(context).ConfigureAwait(false);

                if (message.SystemProperties.LockedUntilUtc <= DateTime.UtcNow)
                    throw new MessageLockExpiredException(_inputAddress, $"The message lock expired: {message.MessageId}");

                if (message.ExpiresAtUtc < DateTime.UtcNow)
                    throw new MessageTimeToLiveExpiredException(_inputAddress, $"The message TTL expired: {message.MessageId}");

                await _receiveEndpointContext.ReceivePipe.Send(context).ConfigureAwait(false);

                await context.ReceiveCompleted.ConfigureAwait(false);

                if (messageReceiver != null)
                    await messageReceiver.CompleteAsync(message.SystemProperties.LockToken).ConfigureAwait(false);

                await _receiveEndpointContext.ReceiveObservers.PostReceive(context).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await _receiveEndpointContext.ReceiveObservers.ReceiveFault(context, ex).ConfigureAwait(false);

                if (messageReceiver == null)
                    throw;

                try
                {
                    await messageReceiver.AbandonAsync(message.SystemProperties.LockToken).ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    if (_log.IsWarnEnabled)
                        _log.Warn($"Abandon message faulted: {message.MessageId}", exception);
                }
            }
            finally
            {
                context.Dispose();
            }
        }

        ConnectHandle IConsumeMessageObserverConnector.ConnectConsumeMessageObserver<T>(IConsumeMessageObserver<T> observer)
        {
            return _receiveEndpointContext.ReceivePipe.ConnectConsumeMessageObserver(observer);
        }

        ConnectHandle IConsumeObserverConnector.ConnectConsumeObserver(IConsumeObserver observer)
        {
            return _receiveEndpointContext.ReceivePipe.ConnectConsumeObserver(observer);
        }
    }
}