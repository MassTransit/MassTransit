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
namespace MassTransit.Containers.Tests.Common_Tests
{
    using System;
    using System.Threading.Tasks;
    using Courier;
    using Courier.Contracts;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Courier;


    public abstract class Courier_ExecuteActivity :
        InMemoryTestFixture
    {
        Task<ConsumeContext<RoutingSlipCompleted>> _completed;
        Task<ConsumeContext<RoutingSlipActivityCompleted>> _activityCompleted;
        Guid _trackingNumber;
        Uri _executeAddress;

        [Test]
        public async Task Should_register_and_execute_the_activity()
        {
            _completed = SubscribeHandler<RoutingSlipCompleted>();
            _activityCompleted = SubscribeHandler<RoutingSlipActivityCompleted>();

            _trackingNumber = NewId.NextGuid();
            var builder = new RoutingSlipBuilder(_trackingNumber);
            builder.AddSubscription(Bus.Address, RoutingSlipEvents.All);

            builder.AddActivity("SetVariableActivity", _executeAddress, new
            {
                Key = "Hello",
                Value = "Hello"
            });

            await Bus.Execute(builder.Build());

            await _completed;
            await _activityCompleted;
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ReceiveEndpoint("execute_testactivity", endpointConfigurator =>
            {
                endpointConfigurator.ConfigureExecuteActivity(Registration, typeof(SetVariableActivity));

                _executeAddress = endpointConfigurator.InputAddress;
            });
        }

        protected abstract IRegistration Registration { get; }
    }


    public abstract class Courier_ExecuteActivity_Endpoint :
        InMemoryTestFixture
    {
        Task<ConsumeContext<RoutingSlipCompleted>> _completed;
        Task<ConsumeContext<RoutingSlipActivityCompleted>> _activityCompleted;
        Guid _trackingNumber;

        [Test]
        public async Task Should_register_and_execute_the_activity()
        {
            _completed = SubscribeHandler<RoutingSlipCompleted>();
            _activityCompleted = SubscribeHandler<RoutingSlipActivityCompleted>();

            _trackingNumber = NewId.NextGuid();
            var builder = new RoutingSlipBuilder(_trackingNumber);
            builder.AddSubscription(Bus.Address, RoutingSlipEvents.All);

            builder.AddActivity("SetVariableActivity", new Uri("loopback://localhost/custom-setvariable-execute"), new
            {
                Key = "Hello",
                Value = "Hello"
            });

            await Bus.Execute(builder.Build());

            await _completed;
            await _activityCompleted;
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ConfigureEndpoints(Registration);
        }

        protected abstract IRegistration Registration { get; }
    }


    public abstract class Courier_Activity :
        InMemoryTestFixture
    {
        Task<ConsumeContext<RoutingSlipCompleted>> _completed;
        Task<ConsumeContext<RoutingSlipActivityCompleted>> _activityCompleted;
        Guid _trackingNumber;
        Uri _executeAddress;

        [Test]
        public async Task Should_register_and_execute_the_activity()
        {
            _completed = SubscribeHandler<RoutingSlipCompleted>();
            _activityCompleted = SubscribeHandler<RoutingSlipActivityCompleted>();

            _trackingNumber = NewId.NextGuid();
            var builder = new RoutingSlipBuilder(_trackingNumber);
            builder.AddSubscription(Bus.Address, RoutingSlipEvents.All);

            builder.AddActivity("TestActivity", _executeAddress, new {Value = "Hello"});

            await Bus.Execute(builder.Build());

            await _completed;
            await _activityCompleted;
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ReceiveEndpoint("execute_testactivity", endpointConfigurator =>
            {
                configurator.ReceiveEndpoint("compensate_testactivity", compensateConfigurator =>
                {
                    endpointConfigurator.ConfigureActivity(compensateConfigurator, Registration, typeof(TestActivity));
                });

                _executeAddress = endpointConfigurator.InputAddress;
            });
        }

        protected abstract IRegistration Registration { get; }
    }


    public abstract class Courier_Activity_Endpoint :
        InMemoryTestFixture
    {
        Task<ConsumeContext<RoutingSlipCompleted>> _completed;
        Task<ConsumeContext<RoutingSlipActivityCompleted>> _activityCompleted;
        Guid _trackingNumber;

        [Test]
        public async Task Should_register_and_execute_the_activity()
        {
            _completed = SubscribeHandler<RoutingSlipCompleted>();
            _activityCompleted = SubscribeHandler<RoutingSlipActivityCompleted>();

            _trackingNumber = NewId.NextGuid();
            var builder = new RoutingSlipBuilder(_trackingNumber);
            builder.AddSubscription(Bus.Address, RoutingSlipEvents.All);

            builder.AddActivity("TestActivity", new Uri("loopback://localhost/custom-testactivity-execute"), new {Value = "Hello"});

            await Bus.Execute(builder.Build());

            await _completed;
            await _activityCompleted;
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ConfigureEndpoints(Registration);
        }

        protected abstract IRegistration Registration { get; }
    }
}
