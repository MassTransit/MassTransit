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
    using GreenPipes;
    using GreenPipes.Agents;
    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;


    public class MessagingFactoryCache :
        PipeContextSupervisor<MessagingFactoryContext>,
        IMessagingFactoryCache
    {
        readonly string _description;

        public MessagingFactoryCache(Uri serviceUri, MessagingFactorySettings settings, RetryPolicy retryPolicy)
            : base(new MessagingFactoryContextFactory(serviceUri, settings, retryPolicy))
        {
            _description = $"MessagingFactoryCache (serviceUri: {serviceUri})";
        }

        public void Probe(ProbeContext context)
        {
            if (HasContext)
                context.Add("connected", true);
        }

        public override string ToString()
        {
            return _description;
        }
    }
}