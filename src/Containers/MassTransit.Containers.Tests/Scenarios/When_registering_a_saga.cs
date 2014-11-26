// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Containers.Tests.Scenarios
{
    using Magnum.TestFramework;


    [Scenario]
    public abstract class When_registering_a_saga :
        Given_a_service_bus_instance
    {
        [When]
        public void Registering_a_saga()
        {
        }

        [Then]
        public void Should_have_a_subscription_for_the_first_saga_message()
        {
        }

        [Then]
        public void Should_have_a_subscription_for_the_second_saga_message()
        {
        }

        [Then]
        public void Should_have_a_subscription_for_the_third_saga_message()
        {
        }
    }
}