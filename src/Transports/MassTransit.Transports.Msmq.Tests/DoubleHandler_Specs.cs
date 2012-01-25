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
namespace MassTransit.Transports.Msmq.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using BusConfigurators;
    using Magnum.Extensions;
    using Magnum.TestFramework;
    using NUnit.Framework;
    using Pipeline.Inspectors;
    using TestFixtures;
    using TestFramework;

    [TestFixture]
    public class When_a_message_is_received_by_two_handlers :
        MsmqEndpointTestFixture
    {
        Future<FirstLevelInterface> _first;
        Future<SecondLevelInterface> _second;
        Future<ThirdLevelInterface> _third;
        Semaphore _receiveEvent;
        IList<Exception> _exceptions;

        public When_a_message_is_received_by_two_handlers()
        {
            _exceptions = new List<Exception>();
            _receiveEvent = new Semaphore(0, 100);

            _first = new Future<FirstLevelInterface>();
            _second = new Future<SecondLevelInterface>();
            _third = new Future<ThirdLevelInterface>();
        }

        public interface FirstLevelInterface
        {
            string Text { get; set; }
        }

        public interface SecondLevelInterface :
            FirstLevelInterface
        {
            int Priority { get; set; }
        }

        public interface ThirdLevelInterface :
            SecondLevelInterface
        {
        }

        public class TopLevelClass :
            ThirdLevelInterface
        {
            public string Text { get; set; }

            public int Priority { get; set; }
        }

        protected override void ConfigureLocalBus(ServiceBusConfigurator configurator)
        {
            base.ConfigureLocalBus(configurator);

            configurator.Subscribe(sc =>
                {
                    sc.Handler<FirstLevelInterface>(msg => ReceiveAndCaptureException(_first, msg));
                    sc.Handler<SecondLevelInterface>(msg => ReceiveAndCaptureException(_second, msg));
                    sc.Handler<ThirdLevelInterface>(msg => ReceiveAndCaptureException(_third, msg));
                });
        }

        void ReceiveAndCaptureException<T>(Future<T> future, T msg)
        {
            _receiveEvent.Release();

            try
            {
                future.Complete(msg);
            }
            catch (Exception ex)
            {
                lock(_exceptions)
                   _exceptions.Add(ex);
            }
        }

        [Test]
        public void It_should_only_be_handled_once()
        {
            LocalBus.InboundPipeline.Trace();

            LocalBus.Publish(new TopLevelClass
                {
                    Priority = 1,
                    Text = "Special"
                });

            _first.WaitUntilCompleted(8.Seconds()).ShouldBeTrue("First");
            _second.WaitUntilCompleted(8.Seconds()).ShouldBeTrue("Second");
            _third.WaitUntilCompleted(8.Seconds()).ShouldBeTrue("Third");

            while (_receiveEvent.WaitOne(8.Seconds()))
                ;

            for (int i = 0; i < _exceptions.Count; i++)
            {
                Console.WriteLine("Exception: {0}", _exceptions[i].Message);
            }

            _exceptions.Count.ShouldEqual(0, "Some messages were received more than once");
        }
    }
}