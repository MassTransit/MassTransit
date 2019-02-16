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
namespace MassTransit.Containers.Tests.Autofac_Tests
{
    using Autofac;
    using Common_Tests;
    using NUnit.Framework;
    using TestFramework.Courier;


    [TestFixture]
    public class AutofacCourier_ExecuteActivity :
        Courier_ExecuteActivity
    {
        readonly IContainer _container;

        public AutofacCourier_ExecuteActivity()
        {
            var builder = new ContainerBuilder();
            builder.AddMassTransit(cfg =>
            {
                cfg.AddExecuteActivity<SetVariableActivity, SetVariableArguments>();
                cfg.AddBus(context => BusControl);
            });

            _container = builder.Build();
        }

        [OneTimeTearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        protected override void ConfigureExecuteActivity(IReceiveEndpointConfigurator endpointConfigurator)
        {
            endpointConfigurator.ConfigureExecuteActivity(_container, typeof(SetVariableActivity));
        }
    }


    [TestFixture]
    public class AutofacCourier_ExecuteActivity_Endpoint :
        Courier_ExecuteActivity_Endpoint
    {
        readonly IContainer _container;

        public AutofacCourier_ExecuteActivity_Endpoint()
        {
            var builder = new ContainerBuilder();
            builder.AddMassTransit(cfg =>
            {
                cfg.AddExecuteActivity<SetVariableActivity, SetVariableArguments>()
                    .Endpoint(e => e.Name = "custom-setvariable-execute");

                cfg.AddBus(context => BusControl);
            });

            _container = builder.Build();
        }

        [OneTimeTearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        protected override void ConfigureEndpoints(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ConfigureEndpoints(_container);
        }
    }


    [TestFixture]
    public class AutofacCourier_Activity :
        Courier_Activity
    {
        readonly IContainer _container;

        public AutofacCourier_Activity()
        {
            var builder = new ContainerBuilder();
            builder.AddMassTransit(cfg =>
            {
                cfg.AddActivity<TestActivity, TestArguments, TestLog>();
                cfg.AddBus(context => BusControl);
            });

            _container = builder.Build();
        }

        [OneTimeTearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        protected override void ConfigureActivity(IReceiveEndpointConfigurator executeEndpointConfigurator,
            IReceiveEndpointConfigurator compensateEndpointConfigurator)
        {
            executeEndpointConfigurator.ConfigureActivity(compensateEndpointConfigurator, _container, typeof(TestActivity));
        }
    }


    [TestFixture]
    public class AutofacCourier_Activity_Endpoint :
        Courier_Activity_Endpoint
    {
        readonly IContainer _container;

        public AutofacCourier_Activity_Endpoint()
        {
            var builder = new ContainerBuilder();
            builder.AddMassTransit(cfg =>
            {
                cfg.AddActivity<TestActivity, TestArguments, TestLog>()
                    .Endpoints(e => e.Name = "custom-testactivity-execute", e => e.Name = "custom-testactivity-compensate");

                cfg.AddBus(context => BusControl);
            });

            _container = builder.Build();
        }

        [OneTimeTearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        protected override void ConfigureEndpoints(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ConfigureEndpoints(_container);
        }
    }
}
