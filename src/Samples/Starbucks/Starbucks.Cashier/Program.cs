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

namespace Starbucks.Cashier
{
    using System;
    using System.Diagnostics;
    using Magnum;
    using Magnum.StateMachine;
    using MassTransit.Log4NetIntegration.Logging;
    using Ninject;
    using Topshelf;

    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Log4NetLogger.Use("cashier.log4net.xml");

            HostFactory.Run(c =>
                {
                    c.SetServiceName("StarbucksCashier");
                    c.SetDisplayName("Starbucks Cashier");
                    c.SetDescription("a Mass Transit sample service for handling orders of coffee.");

                    c.RunAsLocalSystem();
                    c.DependsOnMsmq();

                    var kernel = new StandardKernel();
                    var module = new CashierRegistry();
                    kernel.Load(module);

                    DisplayStateMachine();

                    c.Service<CashierService>(s =>
                        {
                            s.ConstructUsing(builder => kernel.Get<CashierService>());
                            s.WhenStarted(o => o.Start());
                            s.WhenStopped(o => o.Stop());
                        });
                });
        }

        static void DisplayStateMachine()
        {
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));

            StateMachineInspector.Trace(new CashierSaga(CombGuid.Generate()));
        }
    }
}