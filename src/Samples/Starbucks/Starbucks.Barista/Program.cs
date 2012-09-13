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

namespace Starbucks.Barista
{
    using System;
    using System.Diagnostics;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using Magnum;
    using Magnum.StateMachine;
    using MassTransit;
    using MassTransit.Log4NetIntegration.Logging;
    using MassTransit.Saga;
    using Topshelf;

    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Log4NetLogger.Use("barista.log4net.xml");

            var container = new WindsorContainer();
            container.Register(
                Component.For(typeof (ISagaRepository<>))
                    .ImplementedBy(typeof (InMemorySagaRepository<>))
                    .LifeStyle.Singleton,
                Component.For<DrinkPreparationSaga>(),
                Component.For<BaristaService>()
                    .LifeStyle.Singleton,
                Component.For<IServiceBus>().UsingFactoryMethod(() =>
                    {
                        return ServiceBusFactory.New(sbc =>
                            {
                                sbc.ReceiveFrom("msmq://localhost/starbucks_barista");
                                sbc.UseMsmq();
                                sbc.UseMulticastSubscriptionClient();

                                sbc.UseControlBus();

                                sbc.Subscribe(subs => { subs.LoadFrom(container); });
                            });
                    }).LifeStyle.Singleton);

            HostFactory.Run(c =>
                {
                    c.SetServiceName("StarbucksBarista");
                    c.SetDisplayName("Starbucks Barista");
                    c.SetDescription("a Mass Transit sample service for making orders of coffee.");

                    c.DependsOnMsmq();
                    c.RunAsLocalService();

                    DisplayStateMachine();

                    c.Service<BaristaService>(s =>
                        {
                            s.ConstructUsing(builder => container.Resolve<BaristaService>());
                            s.WhenStarted(o => o.Start());
                            s.WhenStopped(o =>
                                {
                                    o.Stop();
                                    container.Dispose();
                                });
                        });
                });
        }

        static void DisplayStateMachine()
        {
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));

            StateMachineInspector.Trace(new DrinkPreparationSaga(CombGuid.Generate()));
        }
    }
}