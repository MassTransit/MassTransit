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
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Contexts;
    using GreenPipes;
    using Transport;


    public abstract class ClientContextFactory :
        JoinContextFactory<NamespaceContext, MessagingFactoryContext, ClientContext>
    {
        readonly ClientSettings _settings;

        protected ClientContextFactory(IMessagingFactoryContextSupervisor messagingFactoryContextSupervisor,
            INamespaceContextSupervisor namespaceContextSupervisor, IPipe<MessagingFactoryContext> messagingFactoryPipe,
            IPipe<NamespaceContext> namespacePipe, ClientSettings settings)
            : base(namespaceContextSupervisor, namespacePipe, messagingFactoryContextSupervisor, messagingFactoryPipe)
        {
            _settings = settings;
        }

        protected override ClientContext CreateClientContext(NamespaceContext leftContext, MessagingFactoryContext rightContext)
        {
            var inputAddress = _settings.GetInputAddress(rightContext.ServiceAddress, _settings.Path);

            return CreateClientContext(rightContext, inputAddress);
        }

        protected abstract ClientContext CreateClientContext(MessagingFactoryContext connectionContext, Uri inputAddress);

        protected override async Task<ClientContext> CreateSharedContext(Task<ClientContext> context, CancellationToken cancellationToken)
        {
            var clientContext = await context.ConfigureAwait(false);

            var sharedContext = new SharedClientContext(clientContext, cancellationToken);

            return sharedContext;
        }
    }
}
