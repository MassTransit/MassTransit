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


    [TestFixture]
    public class DependencyInjection_RequestClient_Context
        : RequestClient_Context
    {
        readonly IServiceProvider _provider;

        public DependencyInjection_RequestClient_Context()
        {
            var collection = new ServiceCollection();
            collection.AddMassTransit(x =>
            {
                x.AddConsumer<InitialConsumer>();
                x.AddConsumer<SubsequentConsumer>();

                x.AddBus(context => BusControl);

                x.AddRequestClient<InitialRequest>(InputQueueAddress);
                x.AddRequestClient<SubsequentRequest>(SubsequentQueueAddress);
            });

            collection.AddSingleton<IConsumeMessageObserver<InitialRequest>>(context => GetConsumeObserver<InitialRequest>());

            _provider = collection.BuildServiceProvider(true);
        }

        protected override IRequestClient<InitialRequest> RequestClient => _provider.CreateRequestClient<InitialRequest>();

        protected override IRegistration Registration => _provider.GetRequiredService<IRegistration>();
    }
}
