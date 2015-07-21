// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.AzureServiceBusTransport.Pipeline
{
    using System.Threading.Tasks;
    using Contexts;
    using MassTransit.Pipeline;
    using Microsoft.ServiceBus;
    using Monitoring.Introspection;


    /// <summary>
    /// Prepares a queue for receiving messages using the ReceiveSettings specified.
    /// </summary>
    public class PrepareReceiveQueueFilter :
        IFilter<ConnectionContext>
    {
        readonly ReceiveSettings _settings;

        public PrepareReceiveQueueFilter(ReceiveSettings settings)
        {
            _settings = settings;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
        }

        public async Task Send(ConnectionContext context, IPipe<ConnectionContext> next)
        {
            NamespaceManager namespaceManager = await context.NamespaceManager;

            await namespaceManager.CreateQueueSafeAsync(_settings.QueueDescription);

            context.GetOrAddPayload(() => _settings);

            await next.Send(context);
        }
    }
}