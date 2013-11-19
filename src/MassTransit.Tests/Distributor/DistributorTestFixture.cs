// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Tests.Distributor
{
    using BusConfigurators;
    using Load.Messages;
    using Magnum;
    using Magnum.Extensions;
    using MassTransit.Transports;
    using NUnit.Framework;
    using TextFixtures;

    [TestFixture, Ignore]
    public class DistributorTestFixture<TTransportFactory> :
        SubscriptionServiceTestFixture<TTransportFactory>
        where TTransportFactory : class, ITransportFactory, new()
    {
        protected override void ConfigureLocalBus(ServiceBusConfigurator configurator)
        {
            configurator.Distributor(d => d.Handler<FirstCommand>());
        }

        protected void AddFirstCommandInstance(string instanceName, string queueName)
        {
            AddInstance(instanceName, queueName, x => x.Worker(d => d.Handler<FirstCommand>(FirstCommandConsumer)));
        }

        void FirstCommandConsumer(IConsumeContext<FirstCommand> context, FirstCommand message)
        {
            ThreadUtil.Sleep(10.Milliseconds());

            var response = new FirstResponse(message.CorrelationId);

            context.Respond(response);
        }
    }
}