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
namespace MassTransit.Tests.Configuration
{
    using System.Linq;
    using MassTransit.Saga.Connectors;
    using NUnit.Framework;
    using Saga;
    using Saga.Messages;
    using Shouldly;


    public class When_a_saga_is_inspected
    {
        SagaConnector<SimpleSaga> _factory;

        [SetUp]
        public void A_consumer_with_consumes_all_interfaces_is_inspected()
        {
            _factory = new SagaConnector<SimpleSaga>();
        }

        [Test]
        public void Should_create_the_builder()
        {
            _factory.ShouldNotBe(null);
        }

        [Test]
        public void Should_have_three_subscription_types()
        {
            _factory.Connectors.Count().ShouldBe(3);
        }

        [Test]
        public void Should_have_an_a()
        {
            _factory.Connectors.First().MessageType.ShouldBe(typeof(InitiateSimpleSaga));
        }

        [Test]
        public void Should_have_a_b()
        {
            _factory.Connectors.Skip(1).First().MessageType.ShouldBe(typeof(CompleteSimpleSaga));
        }

        [Test]
        public void Should_have_a_c()
        {
            _factory.Connectors.Skip(2).First().MessageType.ShouldBe(typeof(ObservableSagaMessage));
        }
    }
}
