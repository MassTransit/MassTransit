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
namespace MassTransit.Containers.Tests.DependencyInjection_Tests
{
    using System;
    using Common_Tests;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using TestFramework.Courier;


    [TestFixture]
    public class DependencyInjectionCourier_ExecuteActivity :
        Courier_ExecuteActivity
    {
        readonly IServiceProvider _container;

        public DependencyInjectionCourier_ExecuteActivity()
        {
            var builder = new ServiceCollection();
            builder.AddMassTransit(cfg =>
            {
                cfg.AddExecuteActivity<SetVariableActivity, SetVariableArguments>();
                cfg.AddBus(context => BusControl);
            });

            _container = builder.BuildServiceProvider(true);
        }

        protected override IRegistration Registration => _container.GetRequiredService<IRegistration>();
    }


    [TestFixture]
    public class DependencyInjectionCourier_ExecuteActivity_Endpoint :
        Courier_ExecuteActivity_Endpoint
    {
        readonly IServiceProvider _container;

        public DependencyInjectionCourier_ExecuteActivity_Endpoint()
        {
            var builder = new ServiceCollection();
            builder.AddMassTransit(cfg =>
            {
                cfg.AddExecuteActivity<SetVariableActivity, SetVariableArguments>()
                    .Endpoint(e => e.Name = "custom-setvariable-execute");

                cfg.AddBus(context => BusControl);
            });

            _container = builder.BuildServiceProvider();
        }

        protected override IRegistration Registration => _container.GetRequiredService<IRegistration>();
    }


    [TestFixture]
    public class DependencyInjectionCourier_Activity :
        Courier_Activity
    {
        readonly IServiceProvider _container;

        public DependencyInjectionCourier_Activity()
        {
            var builder = new ServiceCollection();
            builder.AddMassTransit(cfg =>
            {
                cfg.AddActivity<TestActivity, TestArguments, TestLog>();
                cfg.AddBus(context => BusControl);
            });

            _container = builder.BuildServiceProvider();
        }

        protected override IRegistration Registration => _container.GetRequiredService<IRegistration>();
    }


    [TestFixture]
    public class DependencyInjectionCourier_Activity_Endpoint :
        Courier_Activity_Endpoint
    {
        readonly IServiceProvider _container;

        public DependencyInjectionCourier_Activity_Endpoint()
        {
            var builder = new ServiceCollection();
            builder.AddMassTransit(cfg =>
            {
                cfg.AddActivity<TestActivity, TestArguments, TestLog>()
                    .Endpoints(e => e.Name = "custom-testactivity-execute", e => e.Name = "custom-testactivity-compensate");

                cfg.AddBus(context => BusControl);
            });

            _container = builder.BuildServiceProvider();
        }

        protected override IRegistration Registration => _container.GetRequiredService<IRegistration>();
    }
}
