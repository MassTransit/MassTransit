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

namespace OpenAllNight
{
    using System;
    using System.Threading;
    using Castle.Core;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using MassTransit;
    using MassTransit.Log4NetIntegration.Logging;
    using Testers;

    internal class Program
    {
        static readonly DateTime _startedAt = DateTime.Now;
        static DateTime lastPrint = DateTime.Now;

        static void Main()
        {
            /////setup
            Log4NetLogger.Use("log4net.xml");

            var c = new WindsorContainer();
            c.Register(
                Component.For<SimpleMessageHandler>().ImplementedBy<SimpleMessageHandler>(),
                Component.For<Counter>().Named("counter").LifestyleSingleton().ImplementedBy<Counter>(),
                Component.For<CacheUpdateResponseHandler>().Named("rvaoeuaoe").ImplementedBy<CacheUpdateResponseHandler>().LifestyleTransient()
                );
            
            
            var bus = ServiceBusFactory.New(sbc =>
                                                {
                                                    sbc.UseMsmq();
                                                    sbc.VerifyMsDtcConfiguration();
                                                    sbc.VerifyMsmqConfiguration();
                                                    sbc.ReceiveFrom("msmq://localhost/mt_client");
                                                    sbc.UseSubscriptionService("msmq://localhost/mt_subscriptions");
                                                    sbc.UseHealthMonitoring(10);

                                                    sbc.Subscribe(subs => { subs.LoadFrom(c); });
                                                });

            c.Register(Component.For<IServiceBus>().Instance(bus));


            IEndpoint ep = bus.GetEndpoint(new Uri("msmq://localhost/mt_subscriptions"));
            var subTester = new SubscriptionServiceTester(ep, bus, c.Resolve<Counter>());
            var healthTester = new HealthServiceTester(c.Resolve<Counter>(), bus);
            var timeoutTester = new TimeoutTester(bus);
            bus.SubscribeInstance(healthTester);
            ///////


            Console.WriteLine("Please enter the number of hours you would like this test to run for?");
            Console.WriteLine("(use 0.1 for 6 minutes)");
            Console.WriteLine("(use 0.016 for 1 minute)");
            string input = Console.ReadLine();
            double hours = double.Parse(input ?? "0");
            DateTime stopTime = DateTime.Now.AddHours(hours);


            Console.WriteLine("Test Started");
            var rand = new Random();
            while (DateTime.Now < stopTime)
            {
                subTester.Test();
                healthTester.Test();
                timeoutTester.Test();

                Thread.Sleep(rand.Next(5, 10)*1000);
                PrintTime(bus, c.Resolve<Counter>());
            }

            //print final stuff (probably do this by tester)
            subTester.Results();
            Console.WriteLine("Done (press any key to exit)");
            Console.ReadKey(true);
        }

        static void Initialize()
        {
        }

        static void PrintTime(IServiceBus bus, Counter counter)
        {
            TimeSpan ts = DateTime.Now.Subtract(lastPrint);
            if (ts.Minutes >= 1)
            {
                Console.WriteLine(
                    "Elapsed Time: {0} mins, So far I have - Sent: {1}, Received: {2}, Published: {3}, Received: {4}",
                    (int) ((DateTime.Now - _startedAt).TotalMinutes), counter.MessagesSent, counter.MessagesReceived,
                    counter.PublishCount, SimpleMessageHandler.MessageCount);

                lastPrint = DateTime.Now;
            }
        }
    }


    public class SimpleMessageHandler :
        Consumes<SimpleMessage>.All
    {
        static long _messageCount;

        public static long MessageCount
        {
            get { return _messageCount; }
        }

        #region All Members

        public void Consume(SimpleMessage message)
        {
            Interlocked.Increment(ref _messageCount);
        }

        #endregion
    }

    [Serializable]
    public class SimpleMessage
    {
        readonly string _data = new string('*', 1024);
    }
}