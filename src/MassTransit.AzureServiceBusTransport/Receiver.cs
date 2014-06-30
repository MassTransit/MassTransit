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

namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using MassTransit.Pipeline;
    using Microsoft.ServiceBus.Messaging;
    using Pipeline;

    public class Receiver :
        ReceiverMetrics,
        IDisposable
    {
        readonly CancellationToken _cancellationToken;
        readonly TaskCompletionSource<ReceiverMetrics> _completeTask;
        readonly Uri _inputAddress;
        readonly IPipe<ReceiveContext> _receivePipe;
        int _currentPendingDeliveryCount;
        long _deliveryCount;
        int _maxPendingDeliveryCount;
        CancellationTokenRegistration _registration;

        public Receiver(MessageReceiver messageReceiver, Uri inputAddress, IPipe<ReceiveContext> receivePipe,
            ReceiveSettings receiveSettings, CancellationToken cancellationToken)
        {
            _inputAddress = inputAddress;
            _receivePipe = receivePipe;
            _cancellationToken = cancellationToken;

            _completeTask = new TaskCompletionSource<ReceiverMetrics>();

            var options = new OnMessageOptions
            {
                AutoComplete = false,
                AutoRenewTimeout = receiveSettings.AutoRenewTimeout,
                MaxConcurrentCalls = receiveSettings.MaxConcurrentCalls
            };
            options.ExceptionReceived += (sender, x) => _completeTask.TrySetException(x.Exception);

            messageReceiver.OnMessageAsync(OnMessage, options);

            _registration = cancellationToken.Register(messageReceiver.Close);
        }

        public Task<ReceiverMetrics> CompleteTask
        {
            get { return _completeTask.Task; }
        }

        public void Dispose()
        {
            _registration.Dispose();
        }

        public long DeliveryCount
        {
            get { return _deliveryCount; }
        }

        public int ConcurrentDeliveryCount
        {
            get { return _maxPendingDeliveryCount; }
        }

        public async Task OnMessage(BrokeredMessage message)
        {
            Interlocked.Increment(ref _deliveryCount);

            int current = Interlocked.Increment(ref _currentPendingDeliveryCount);
            while (current > _maxPendingDeliveryCount)
                Interlocked.CompareExchange(ref _maxPendingDeliveryCount, current, _maxPendingDeliveryCount);

            try
            {
                var context = new AzureServiceBusReceiveContext(message, _inputAddress);

                await _receivePipe.Send(context);

                await message.CompleteAsync();

                Interlocked.Decrement(ref _currentPendingDeliveryCount);
            }
            catch (Exception ex)
            {
                message.AbandonAsync().Wait(_cancellationToken);

                Interlocked.Decrement(ref _currentPendingDeliveryCount);
            }
        }
    }
}