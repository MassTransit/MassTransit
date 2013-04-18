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
namespace MassTransit.Tests.Saga
{
    using BusConfigurators;
    using MassTransit.Saga;
    using NUnit.Framework;
    using TextFixtures;


    [TestFixture]
    public class Injecting_properties_into_a_saga :
        LoopbackTestFixture
    {
        protected override void ConfigureLocalBus(ServiceBusConfigurator configurator)
        {
            base.ConfigureLocalBus(configurator);

            // this is our dependency, but could be dynamically resolved from a container in method
            // below is so desired.
            var dependency = new Dependency();

            // create the actual saga repository
            ISagaRepository<InjectingSampleSaga> sagaRepository = SetupSagaRepository<InjectingSampleSaga>();

            // decorate the saga repository with the injecting repository, specifying the property and a
            // lambda method to return the property value given the saga instance that was loaded
            // allows properties of the saga to be used in the resolution of the dependency
            ISagaRepository<InjectingSampleSaga> injectingRepository =
                InjectingSagaRepository<InjectingSampleSaga>.Create(sagaRepository,
                    x => x.Dependency, saga => dependency);

            // subscribe the decorated saga repository to the bus during configuration
            configurator.Subscribe(x => x.Saga(injectingRepository));
        }


        class Dependency :
            IDependency
        {
        }
    }
}