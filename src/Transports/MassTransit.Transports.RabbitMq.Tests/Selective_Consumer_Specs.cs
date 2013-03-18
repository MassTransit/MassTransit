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
namespace MassTransit.Transports.RabbitMq.Tests
{
    using System;
    using System.Linq;
    using System.Threading;
    using BusConfigurators;
    using Logging;
    using Magnum.Caching;
    using Magnum.Extensions;
    using Magnum.TestFramework;
    using NUnit.Framework;

    public interface Sometimes
    {
        Guid Id { get; }
        bool Accept { get; }
    }

    class SelectiveConsumer : 
        Consumes<Sometimes>.Selected
    {
        static readonly ILog _log = Logger.Get(typeof(SelectiveConsumer));

        Cache<Guid, int> _consumed; 
        Cache<Guid, int> _observed;
        public const int IgnoreCount = 3;

        public SelectiveConsumer()
        {
            _consumed = new ConcurrentCache<Guid, int>();
            _observed = new ConcurrentCache<Guid, int>();
        }

        public int Consumed
        {
            get { return _consumed.Sum(); }
        }

        public int Observed
        {
            get { return _observed.Sum(); }
        }

        public void Consume(Sometimes message)
        {
            _log.Info("Consuming");
            _consumed[message.Id] = _consumed.Get(message.Id, x => 0) + 1;
        }

        public bool Accept(Sometimes message)
        {
            _observed[message.Id] = _observed.Get(message.Id, x => 0) + 1;

            _log.Info(string.Format("Accepting: {0}", message.Accept));

            return message.Accept;
        }
    }

    [Scenario]
    public class Selective_Consumer_Specs
        : Given_a_rabbitmq_bus
    {
        SelectiveConsumer _consumer;
        const int TotalMsgs = 10;

        protected override void ConfigureServiceBus(Uri uri, ServiceBusConfigurator configurator)
        {
            base.ConfigureServiceBus(uri, configurator);

            _consumer = new SelectiveConsumer();
            configurator.Subscribe(s => s.Instance(_consumer));
        }

        [When]
        public void A_message_is_published()
        {
            for (var i = 0; i < TotalMsgs; i++)
            {
                LocalBus.Publish<Sometimes>(new
                    {
                        Id = NewId.NextGuid(),
                        Accept = i % 2 == 0,
                    });
            }
        }

        [Then]
        public void Should_have_observed_each_message_at_least_twice()
        {
            for (int i = 0; i < 80; i++)
            {
                if (_consumer.Observed == TotalMsgs / 2 * 6)
                    return;

                Thread.Sleep(100);
            }

            Assert.Fail("should have observed messages, discarding every other one!");
        }

        [Then]
        public void Should_have_consumed_all_messages()
        {
            for (int i = 0; i < 80; i++)
            {
                if (_consumer.Consumed == TotalMsgs / 2)
                    return;

                Thread.Sleep(100);
            }

            Assert.Fail("should have consumed two messages, skipping the one in the middle!");
        }
    }
}