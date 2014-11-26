// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Tests.Distributor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using BusConfigurators;
    using Load;
    using Load.Messages;
    using Magnum.Extensions;
    using MassTransit.Distributor;
    using MassTransit.Distributor.Messages;
    using MassTransit.Pipeline.Inspectors;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class Default_distributor_specifications :
        DistributorTestFixture
    {
        [Test]
        public void Can_collect_iworkeravaiable_messages()
        {
            int workerAvaiableCountRecieved = 0;
            var messageRecieved = new ManualResetEvent(false);

            ConnectHandle unsubscribe = LocalBus.SubscribeHandler<IWorkerAvailable>(message =>
            {
                Interlocked.Increment(ref workerAvaiableCountRecieved);
                messageRecieved.Set();
            });

            Instances.ToList().ForEach(
                x => { x.Value.DataBus.Endpoint.Send(new PingWorker(), c => { c.SendResponseTo(LocalBus); }); });

            messageRecieved.WaitOne(8.Seconds());

            unsubscribe.Dispose();

            workerAvaiableCountRecieved.ShouldBeGreaterThan(0);
        }

        [Test, Explicit]
        public void Ensure_distributor_sends_ping_request_after_timeout()
        {
            int pingRequestsRecieved = 0;
            var messageRecieved = new ManualResetEvent(false);

            ConnectHandle unsubscribe = Instances["A"].DataBus.SubscribeHandler<PingWorker>(message =>
            {
                Interlocked.Increment(ref pingRequestsRecieved);
                messageRecieved.Set();
            });

            messageRecieved.WaitOne(120.Seconds());

            unsubscribe.Dispose();

            pingRequestsRecieved.ShouldBeGreaterThan(0);
        }

        [Test]
        public void Ensure_workers_will_respond_to_ping_request()
        {
            int workerAvaiableCountRecieved = 0;
            var messageRecieved = new ManualResetEvent(false);

            ConnectHandle unsubscribe = LocalBus.SubscribeHandler<WorkerAvailable<FirstCommand>>(message =>
            {
                Interlocked.Increment(ref workerAvaiableCountRecieved);
                messageRecieved.Set();
            });

            Instances.ToList().ForEach(x =>
            {
                x.Value.DataBus.Endpoint.Send(new PingWorker(),
                    y => y.SendResponseTo(LocalBus));
            });

            messageRecieved.WaitOne(8.Seconds());

            unsubscribe.Dispose();

            workerAvaiableCountRecieved.ShouldBeGreaterThan(0);
        }

        [Test, Explicit]
        public void Using_the_load_generator_should_share_the_load()
        {
            var generator = new LoadGenerator<FirstCommand, FirstResponse>();
            const int count = 100;

            generator.Run(RemoteBus, LocalBus.Endpoint, Instances.Values.Select(x => x.DataBus), count,
                x => new FirstCommand(x));

            Dictionary<Uri, int> results = generator.GetWorkerLoad();

            Assert.That(results.Sum(x => x.Value), Is.EqualTo(count));
            results.ToList().ForEach(x =>
                Assert.That(x.Value, Is.GreaterThan(0).And.LessThanOrEqualTo(count),
                    string.Format("{0} did not consume between 0 and {1}",
                        x.Key.ToString(), count)));
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();

            AddFirstCommandInstance("A", "loopback://localhost/a");
            AddFirstCommandInstance("B", "loopback://localhost/b");
            AddFirstCommandInstance("C", "loopback://localhost/c");

            RemoteBus.ShouldHaveRemoteSubscriptionFor<Distributed<FirstCommand>>();
        }
    }


    [TestFixture]
    public class Distributor_with_custom_worker_selection_strategy :
        DistributorTestFixture
    {
        [Test, Explicit]
        public void Node_a_should_recieve_all_the_work()
        {
            var generator = new LoadGenerator<FirstCommand, FirstResponse>();
            const int count = 100;

            generator.Run(RemoteBus, LocalBus.Endpoint, Instances.Values.Select(x => x.DataBus), count,
                x => new FirstCommand(x));

            Dictionary<Uri, int> results = generator.GetWorkerLoad();

            Assert.That(results.Sum(x => x.Value), Is.EqualTo(count));
            Assert.That(results[_nodes["A"]], Is.EqualTo(count));
        }

        Dictionary<String, Uri> _nodes = new Dictionary<string, Uri>
        {
            {"A", new Uri("loopback://localhost/a")},
            {"B", new Uri("loopback://localhost/b")},
            {"C", new Uri("loopback://localhost/c")}
        };

        protected override void EstablishContext()
        {
            base.EstablishContext();

            _nodes.ToList().ForEach(x => AddFirstCommandInstance(x.Key, x.Value.ToString()));
        }

        protected override void ConfigureLocalBus(ServiceBusConfigurator configurator)
        {
            var selectorFactory = new CustomWorkerSelectorFactory(_nodes["A"]);

            configurator.Distributor(x => x.Handler<FirstCommand>().UseWorkerSelector(() => selectorFactory));
        }


        class CustomWorkerSelectorFactory :
            IWorkerSelectorFactory
        {
            readonly Uri _node;

            public CustomWorkerSelectorFactory(Uri node)
            {
                _node = node;
            }

            public IWorkerSelector<TMessage> GetSelector<TMessage>()
                where TMessage : class
            {
                return new CustomWorkerSelector<TMessage>(_node);
            }
        }


        class CustomWorkerSelector<TMessage> :
            IWorkerSelector<TMessage>
            where TMessage : class
        {
            readonly Uri _node;

            public CustomWorkerSelector(Uri node)
            {
                _node = node;
            }


            public IEnumerable<IWorkerInfo<TMessage>> SelectWorker(IEnumerable<IWorkerInfo<TMessage>> availableWorkers,
                ConsumeContext<TMessage> context)
            {
                return availableWorkers.Where(x => x.DataUri == _node);
            }
        }
    }
}