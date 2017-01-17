// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using GreenPipes;
    using Logging;
    using NewIdFormatters;
    using Transport;


    /// <summary>
    /// Prepares a queue for receiving messages using the ReceiveSettings specified.
    /// </summary>
    public class PrepareReceiveEndpointFilter :
        IFilter<NamespaceContext>
    {
        static readonly ILog _log = Logger.Get<PrepareReceiveEndpointFilter>();

        static readonly INewIdFormatter _formatter = new ZBase32Formatter();
        readonly ReceiveSettings _settings;

        public PrepareReceiveEndpointFilter(ReceiveSettings settings)
        {
            _settings = settings;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            context.CreateFilterScope("prepareReceiveEndpoint");
        }

        public async Task Send(NamespaceContext context, IPipe<NamespaceContext> next)
        {
            context.GetOrAddPayload(() => _settings);

            await next.Send(context).ConfigureAwait(false);
        }
    }
}