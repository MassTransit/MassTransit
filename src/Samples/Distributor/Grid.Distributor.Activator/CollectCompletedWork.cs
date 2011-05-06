// Copyright 2007-2008 The Apache Software Foundation.
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
namespace Grid.Distributor.Activator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Shared;
    using Shared.Messages;
    using log4net;
    using MassTransit;
    using MassTransit.Distributor;
    using System.Configuration;

	public class CollectCompletedWork :
        Consumes<CompletedSimpleWorkItem>.All,
        IServiceInterface
    {
        private UnsubscribeAction _unsubscribeAction;
        private readonly ILog _log = LogManager.GetLogger(typeof(CollectCompletedWork));
        private int _received;
        private int _sent;
        private readonly List<int> _values = new List<int>();

		public IServiceBus ControlBus { get; set; }
        public IServiceBus DataBus { get; set; }

        public CollectCompletedWork()
        {
        	DataBus = ServiceBusFactory.New(x =>
        		{
					x.ReceiveFrom(ConfigurationManager.AppSettings["SourceQueue"]);
        			x.SetPurgeOnStartup(true);

					x.UseMsmq();
        			x.UseMulticastSubscriptionClient();

					x.SetConcurrentConsumerLimit(4);
	                x.UseDistributorFor<DoSimpleWorkItem>();
        			x.UseControlBus();
        		});

        	ControlBus = DataBus.ControlBus;
        }

        public void Consume(CompletedSimpleWorkItem message)
        {
            Interlocked.Increment(ref _received);

            int messageMs = DateTime.UtcNow.Subtract(message.RequestCreatedAt).Milliseconds;

        	int max;
        	int min;
        	double average;
        	lock (_values)
            {
                _values.Add(messageMs);
            	min = _values.Min();
            	max = _values.Max();
            	average = _values.Average();
            }

            _log.InfoFormat("Received: {0} - {1} [{2}ms]", _received, message.CorrelationId, messageMs);
        	_log.InfoFormat("Stats\n\tMin: {0:0000.0}ms\n\tMax: {1:0000.0}ms\n\tAvg: {2:0000.0}ms", min, max, average);
        }

        public void Start()
        {
            _unsubscribeAction = DataBus.SubscribeInstance(this);

            Thread.Sleep(5000);

        	DataBus.OutboundPipeline.View(Console.WriteLine);

			Thread.Sleep(5000);
			Thread.Sleep(5000);
			Thread.Sleep(5000);

            for (int i = 0; i < 100; i++)
            {
                var g = Guid.NewGuid();
                _log.InfoFormat("Publishing: {0}", g);
                DataBus.Publish(new DoSimpleWorkItem(g));

                Interlocked.Increment(ref _sent);
            }
        }

        public void Stop()
        {
            var action = _unsubscribeAction;
            if (action != null)
            {
                action();
            }
        }
    }
}