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
namespace MassTransit.WebJobs.ServiceBusIntegration.Configuration
{
    using System.Threading;
    using AzureServiceBusTransport.Configurators;
    using AzureServiceBusTransport.Specifications;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Host;
    using Topology;
    using Transports;


    public class WebJobBrokeredMessageReceiverSpecification :
        BrokeredMessageReceiverSpecification,
        IWebJobBrokeredMessageReceiverConfigurator,
        IWebJobHandlerFactory
    {
        readonly IBinder _binder;
        readonly IServiceBusEndpointConfiguration _endpointConfiguration;
        CancellationToken _cancellationToken;

        public WebJobBrokeredMessageReceiverSpecification(IBinder binder, IServiceBusEndpointConfiguration endpointConfiguration,
            CancellationToken cancellationToken = default(CancellationToken))
            : base(endpointConfiguration)
        {
            _binder = binder;
            _endpointConfiguration = endpointConfiguration;
            _cancellationToken = cancellationToken;
        }

        public CancellationToken CancellationToken
        {
            set => _cancellationToken = value;
        }

        public void SetLog(TraceWriter traceWriter)
        {
            Log = new TraceWriterLog(traceWriter);

            ReceiveEndpointLoggingExtensions.SetLog(Log);
        }

        protected override IReceiveEndpointTopology CreateReceiveTopology()
        {
            return new WebJobMessageReceiverEndpointTopology(_endpointConfiguration, InputAddress, Log, _binder, _cancellationToken);
        }
    }
}