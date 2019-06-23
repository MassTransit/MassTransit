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
namespace MassTransit.AzureServiceBusTransport.Pipeline
{
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Contexts;
    using GreenPipes;
    using Transport;


    public class TopicSendEndpointContextFactory :
        JoinContextFactory<NamespaceContext, MessagingFactoryContext, SendEndpointContext>
    {
        readonly SendSettings _settings;

        public TopicSendEndpointContextFactory(IMessagingFactoryContextSupervisor messagingFactoryContextSupervisor,
            INamespaceContextSupervisor namespaceContextSupervisor, IPipe<MessagingFactoryContext> messagingFactoryPipe,
            IPipe<NamespaceContext> namespacePipe, SendSettings settings)
            : base(namespaceContextSupervisor, namespacePipe, messagingFactoryContextSupervisor, messagingFactoryPipe)
        {
            _settings = settings;
        }

        protected override SendEndpointContext CreateClientContext(NamespaceContext leftContext, MessagingFactoryContext rightContext)
        {
            LogContext.Debug?.Log("Creating Topic Client: {Queue}", _settings.EntityPath);

            var topicClient = rightContext.MessagingFactory.CreateTopicClient(_settings.EntityPath);

            return new TopicSendEndpointContext(topicClient);
        }

        protected override async Task<SendEndpointContext> CreateSharedContext(Task<SendEndpointContext> context, CancellationToken cancellationToken)
        {
            var sendEndpointContext = await context.ConfigureAwait(false);

            return new SharedSendEndpointContext(sendEndpointContext, cancellationToken);
        }
    }
}
