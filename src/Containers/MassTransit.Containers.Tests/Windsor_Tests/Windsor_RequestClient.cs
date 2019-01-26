// Copyright 2007-2019 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
// Copyright 2007-2019 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Containers.Tests.Windsor_Tests
{
    using Castle.Windsor;
    using Common_Tests;
    using NUnit.Framework;


    [TestFixture]
    public class Windsor_RequestClient_Context
        : RequestClient_Context
    {
        readonly IWindsorContainer _container;

        public Windsor_RequestClient_Context()
        {
            _container = new WindsorContainer();
            _container.AddMassTransit(x =>
            {
                x.AddConsumer<InitialConsumer>();
                x.AddConsumer<SubsequentConsumer>();

                x.AddBus(context => BusControl);

                x.AddRequestClient<InitialRequest>(InputQueueAddress);
                x.AddRequestClient<SubsequentRequest>(SubsequentQueueAddress);
            });
        }

        protected override IRequestClient<InitialRequest> RequestClient => _container.Resolve<IRequestClient<InitialRequest>>();

        protected override void ConfigureInitialConsumer(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureConsumer<InitialConsumer>(_container);
        }

        protected override void ConfigureSubsequentConsumer(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureConsumer<SubsequentConsumer>(_container);
        }
    }
}