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
namespace MassTransit.AzureServiceBusTransport.Contexts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using GreenPipes.Payloads;
    using Logging;
    using Microsoft.ServiceBus.Messaging;


    public class ServiceBusMessagingFactoryContext :
        BasePipeContext,
        MessagingFactoryContext,
        IAsyncDisposable
    {
        static readonly ILog _log = Logger.Get<ServiceBusMessagingFactoryContext>();

        readonly MessagingFactory _messagingFactory;

        public ServiceBusMessagingFactoryContext(MessagingFactory messagingFactory, CancellationToken cancellationToken)
            : base(new PayloadCache(), cancellationToken)
        {
            _messagingFactory = messagingFactory;
        }

        public async Task DisposeAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Closing messaging factory: {0}", _messagingFactory.Address);

            await _messagingFactory.CloseAsync().ConfigureAwait(false);

            if (_log.IsDebugEnabled)
                _log.DebugFormat("Closed messaging factory: {0}", _messagingFactory.Address);
        }

        MessagingFactory MessagingFactoryContext.MessagingFactory => _messagingFactory;

        public Uri ServiceAddress => _messagingFactory.Address;
    }
}