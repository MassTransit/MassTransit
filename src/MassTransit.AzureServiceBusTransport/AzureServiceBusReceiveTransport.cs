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

        public AzureServiceBusReceiveTransport(ServiceBusHostSettings hostSettings, ReceiveSettings settings, IRetryPolicy retryPolicy)
        {
            _hostSettings = hostSettings;
            _settings = settings;
            _retryPolicy = retryPolicy;
        }

        public Task Start(IPipe<ReceiveContext> receivePipe, CancellationToken stopReceive)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Starting receive transport: {0}", new Uri(_hostSettings.ServiceUri, _settings.QueueDescription.Path));

            IPipe<ConnectionContext> connectionPipe = Pipe.New<ConnectionContext>(x =>
            {
                x.Repeat(stopReceive);
                x.Retry(_retryPolicy, stopReceive);

                x.Filter(new PrepareReceiveQueueFilter(_settings));
                x.Filter(new MessageReceiverFilter(receivePipe));
            });

            return Repeat.UntilCancelled(stopReceive, async () =>
            {
                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Connecting receive transport: {0}", new Uri(_hostSettings.ServiceUri, _settings.QueueDescription.Path));

                try
                {
                    using (var context = new ServiceBusConnectionContext(_hostSettings, stopReceive))
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
        }
    }
}