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
    using Context;
    using Internals.Extensions;
    using Logging;
    using MassTransit.Pipeline;
    using Pipeline;
    using Policies;
    using Transports;


    public class AzureServiceBusReceiveTransport :
        IReceiveTransport
    {
        readonly ServiceBusHostSettings _hostSettings;
        readonly ILog _log = Logger.Get<AzureServiceBusReceiveTransport>();
        readonly IRetryPolicy _retryPolicy;
        readonly ReceiveSettings _settings;
        Uri _inputAddress;

        public AzureServiceBusReceiveTransport(ServiceBusHostSettings hostSettings, ReceiveSettings settings, IRetryPolicy retryPolicy)
        {
            _hostSettings = hostSettings;
            _settings = settings;
            _retryPolicy = retryPolicy;
        }

        public Uri InputAddress
        {
            get { return _inputAddress; }
        }

        public async Task<ReceiveTransportHandle> Start(IPipe<ReceiveContext> receivePipe, CancellationToken cancellationToken)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Starting receive transport: {0}", new Uri(_hostSettings.ServiceUri, _settings.QueueDescription.Path));

            var handle = new Handle(this);

            IPipe<ConnectionContext> connectionPipe = Pipe.New<ConnectionContext>(x =>
            {
                x.Repeat(handle.StopToken);
                x.Retry(_retryPolicy, handle.StopToken);

                x.Filter(new PrepareReceiveQueueFilter(_settings));
                x.Filter(new MessageReceiverFilter(receivePipe));
            });

            Receiver(handle, connectionPipe);

            return handle;
        }

        async void Receiver(Handle handle, IPipe<ConnectionContext> connectionPipe)
        {
            await Repeat.UntilCancelled(handle.StopToken, async () =>
            {
                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Connecting receive transport: {0}", new Uri(_hostSettings.ServiceUri, _settings.QueueDescription.Path));

                try
                {
                    using (var context = new ServiceBusConnectionContext(_hostSettings, handle.StopToken))
                    {
                        await connectionPipe.Send(context);
                    }
                }
                catch (TaskCanceledException)
                {
                }
                catch (Exception ex)
                {
                    if (_log.IsErrorEnabled)
                        _log.ErrorFormat("Azure Service Bus connection failed: {0}", ex.Message);
                }
            });

            handle.Stopped();
        }


        class Handle :
            ReceiveTransportHandle
        {
            readonly CancellationTokenSource _stop;
            readonly IReceiveTransport _transport;
            readonly TaskCompletionSource<bool> _stopped;

            public Handle(IReceiveTransport transport)
            {
                _transport = transport;
                _stop = new CancellationTokenSource();
                _stopped = new TaskCompletionSource<bool>();
            }

            void IDisposable.Dispose()
            {
                _stop.Cancel();
            }

            IReceiveTransport ReceiveTransportHandle.Transport
            {
                get { return _transport; }
            }

            async Task ReceiveTransportHandle.Stop(CancellationToken cancellationToken)
            {
                _stop.Cancel();

                await _stopped.Task.WithCancellation(cancellationToken);
            }

            public CancellationToken StopToken
            {
                get { return _stop.Token; }
            }

            public void Stopped()
            {
                _stopped.TrySetResult(true);
            }
        }
    }
}