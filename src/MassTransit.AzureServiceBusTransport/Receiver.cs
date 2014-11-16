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
    using Logging;
    using MassTransit.Pipeline;
    using Microsoft.ServiceBus.Messaging;


    public class Receiver :
        ReceiverMetrics,
        IDisposable
    {
        static readonly ILog _log = Logger.Get<Receiver>();

        readonly CancellationToken _cancellationToken;
        readonly TaskCompletionSource<ReceiverMetrics> _completeTask;
        readonly Uri _inputAddress;
        readonly MessageReceiver _messageReceiver;
        readonly IPipe<ReceiveContext> _receivePipe;
        readonly ReceiveSettings _receiveSettings;
        int _currentPendingDeliveryCount;
        long _deliveryCount;
        int _maxPendingDeliveryCount;
        CancellationTokenRegistration _registration;
        bool _shuttingDown;

        public Receiver(MessageReceiver messageReceiver, Uri inputAddress, IPipe<ReceiveContext> receivePipe,
            ReceiveSettings receiveSettings, CancellationToken cancellationToken)
        {
            _messageReceiver = messageReceiver;
            _inputAddress = inputAddress;
            _receivePipe = receivePipe;
            _receiveSettings = receiveSettings;
            _cancellationToken = cancellationToken;

            _completeTask = new TaskCompletionSource<ReceiverMetrics>();

            _registration = cancellationToken.Register(() =>
            {
                Shutdown();
            });

            var options = new OnMessageOptions
            {
                AutoComplete = false,
                AutoRenewTimeout = receiveSettings.AutoRenewTimeout,
                MaxConcurrentCalls = receiveSettings.MaxConcurrentCalls,
            };

            options.ExceptionReceived += (sender, x) => _completeTask.TrySetException(x.Exception);

            messageReceiver.OnMessageAsync(OnMessage, options);
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

        void Shutdown()
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Shutting down receiver: {0}", _inputAddress);

            _shuttingDown = true;

            try
            {
                _messageReceiver.Close();
            }
            catch (Exception ex)
            {
                _completeTask.TrySetException(ex);
            }
            finally
            {
                if (_currentPendingDeliveryCount == 0)
                    _completeTask.TrySetResult(this);
            }
        }

        async Task OnMessage(BrokeredMessage message)
        {
            long deliveryCount = Interlocked.Increment(ref _deliveryCount);

            int current = Interlocked.Increment(ref _currentPendingDeliveryCount);
            while (current > _maxPendingDeliveryCount)
                Interlocked.CompareExchange(ref _maxPendingDeliveryCount, current, _maxPendingDeliveryCount);

            if (_log.IsDebugEnabled)
                _log.DebugFormat("Recieving {0}:{1} - {2}", deliveryCount, message.MessageId, _receiveSettings.QueueDescription.Path);

            Exception exception = null;
            var context = new AzureServiceBusReceiveContext(message, _inputAddress);

            try
            {
                await _receivePipe.Send(context);

                await message.CompleteAsync();

                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Receive completed: {0}", message.MessageId);
            }
            catch (Exception ex)
            {
                exception = ex;
                if (_log.IsErrorEnabled)
                    _log.Error(string.Format("Received faulted: {0}", message.MessageId), ex);
            }

            try
            {
                if (exception != null)
                    await message.AbandonAsync();
            }
            finally
            {
                int pendingCount = Interlocked.Decrement(ref _currentPendingDeliveryCount);
                if (pendingCount == 0 && _shuttingDown)
                    _completeTask.TrySetResult(this);
            }
        }
    }
}