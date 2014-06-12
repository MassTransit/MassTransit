// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Tests
{
    using System;
    using BusConfigurators;
    using Magnum.Extensions;
    using Magnum.TestFramework;
    using TextFixtures;

    [Scenario]
    public class When_publishing_an_interface_message :
        LoopbackTestFixture
    {
        FutureMessage<IProxyMe> _received;
        int _intValue = 42;
        string _stringValue = "Hello";
        Guid _correlationId = Guid.NewGuid();

        protected override void EstablishContext()
        {
            base.EstablishContext();

            LocalBus.Publish<IProxyMe>(new {IntValue = _intValue, StringValue = _stringValue, CorrelationId = _correlationId});

            _received.IsAvailable(8.Seconds());
        }

        protected override void ConfigureLocalBus(ServiceBusConfigurator configurator)
        {
            base.ConfigureLocalBus(configurator);

            _received = new FutureMessage<IProxyMe>();

            configurator.Subscribe(x => { x.Handler<IProxyMe>(async context => _received.Set(context.Message)); });
        }

        [Then]
        public void Should_have_received_message()
        {
            _received.IsAvailable(TimeSpan.Zero).ShouldBeTrue();
        }

        [Then]
        public void Should_have_integer_value()
        {
            _received.Message.IntValue.ShouldEqual(_intValue);
        }

        [Then]
        public void Should_have_string_value()
        {
            _received.Message.StringValue.ShouldEqual(_stringValue);
        }

        [Then]
        public void Should_have_correlation_id()
        {
            _received.Message.CorrelationId.ShouldEqual(_correlationId);
        }

        public interface IProxyMe :
            CorrelatedBy<Guid>
        {
            int IntValue { get; }
            string StringValue { get; }
        }
    }

    [Scenario]
    public class When_sending_an_interface_message :
        LoopbackTestFixture
    {
        FutureMessage<IProxyMe> _received;
        int _intValue = 42;
        string _stringValue = "Hello";
        Guid _correlationId = Guid.NewGuid();

        protected override void EstablishContext()
        {
            base.EstablishContext();

            LocalBus.Endpoint.Send<IProxyMe>(new {IntValue = _intValue, StringValue = _stringValue, CorrelationId = _correlationId});

            _received.IsAvailable(8.Seconds());
        }

        protected override void ConfigureLocalBus(ServiceBusConfigurator configurator)
        {
            base.ConfigureLocalBus(configurator);

            _received = new FutureMessage<IProxyMe>();

            configurator.Subscribe(x => { x.Handler<IProxyMe>(async context => _received.Set(context.Message)); });
        }

        [Then]
        public void Should_have_received_message()
        {
            _received.IsAvailable(TimeSpan.Zero).ShouldBeTrue();
        }

        [Then]
        public void Should_have_integer_value()
        {
            _received.Message.IntValue.ShouldEqual(_intValue);
        }

        [Then]
        public void Should_have_string_value()
        {
            _received.Message.StringValue.ShouldEqual(_stringValue);
        }

        [Then]
        public void Should_have_correlation_id()
        {
            _received.Message.CorrelationId.ShouldEqual(_correlationId);
        }

        public interface IProxyMe :
            CorrelatedBy<Guid>
        {
            int IntValue { get; }
            string StringValue { get; }
        }
    }
}