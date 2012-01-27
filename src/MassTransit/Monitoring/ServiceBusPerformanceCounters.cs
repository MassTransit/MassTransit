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
namespace MassTransit.Monitoring
{
	using System.Diagnostics;
	using System.Linq;
	using System.Security;
	using Logging;
	using Magnum.Extensions;

    public class ServiceBusPerformanceCounters
	{
	    public const string CategoryName = "MassTransit";
	    const string CategoryHelp = "MassTransit Performance Counters";
	    static readonly ServiceBusPerformanceCounters _instance;
		static readonly ILog _log = Logger.Get(typeof (ServiceBusPerformanceCounters));

		static ServiceBusPerformanceCounters()
		{
			_instance = new ServiceBusPerformanceCounters();
		}

		ServiceBusPerformanceCounters()
		{
			ConsumerThreadCount = new RuntimePerformanceCounter("Consumer Threads",
				"The current number of threads processing messages.",
				PerformanceCounterType.NumberOfItems32);

			ReceiveThreadCount = new RuntimePerformanceCounter("Receive Threads",
				"The current number of threads receiving messages.",
				PerformanceCounterType.NumberOfItems32);

			ReceiveRate = new RuntimePerformanceCounter("Received/s",
				"The number of messages received per second",
				PerformanceCounterType.RateOfCountsPerSecond32);

			PublishRate = new RuntimePerformanceCounter("Published/s",
				"The number of messages published per second.",
				PerformanceCounterType.RateOfCountsPerSecond32);

			SendRate = new RuntimePerformanceCounter("Sent/s",
				"The number of messages sent per second.",
				PerformanceCounterType.RateOfCountsPerSecond32);

			ReceiveCount = new RuntimePerformanceCounter("Messages Received",
				"The total number of message received.",
				PerformanceCounterType.NumberOfItems32);

			PublishCount = new RuntimePerformanceCounter("Messages Published",
				"The total number of message published.",
				PerformanceCounterType.NumberOfItems32);

			SentCount = new RuntimePerformanceCounter("Messages Sent",
				"The total number of message sent.",
				PerformanceCounterType.NumberOfItems32);

			ConsumerDuration = new RuntimePerformanceCounter("Average Consumer Duration",
				"The average time a consumer spends processing a message.",
				PerformanceCounterType.AverageCount64);

			ConsumerDurationBase = new RuntimePerformanceCounter("Average Consumer Duration Base",
				"The average time a consumer spends processing a message.",
				PerformanceCounterType.AverageBase);

			ReceiveDuration = new RuntimePerformanceCounter("Average Receive Duration",
				"The average time to receive a message.",
				PerformanceCounterType.AverageCount64);

			ReceiveDurationBase = new RuntimePerformanceCounter("Average Receive Duration Base",
				"The average time to receive a message.",
				PerformanceCounterType.AverageBase);

			PublishDuration = new RuntimePerformanceCounter("Average Publish Duration",
				"The average time to Publish a message.",
				PerformanceCounterType.AverageCount64);

			PublishDurationBase = new RuntimePerformanceCounter("Average Publish Duration Base",
				"The average time to Publish a message.",
				PerformanceCounterType.AverageBase);

			InitiatizeCategory();
		}

		public static ServiceBusPerformanceCounters Instance
		{
			get { return _instance; }
		}

		public RuntimePerformanceCounter ConsumerDuration { get; private set; }
		public RuntimePerformanceCounter ConsumerDurationBase { get; private set; }

		public RuntimePerformanceCounter ConsumerThreadCount { get; private set; }
		public RuntimePerformanceCounter PublishCount { get; private set; }
		public RuntimePerformanceCounter PublishDuration { get; private set; }
		public RuntimePerformanceCounter PublishDurationBase { get; private set; }
		public RuntimePerformanceCounter PublishRate { get; private set; }

		public RuntimePerformanceCounter ReceiveCount { get; private set; }

		public RuntimePerformanceCounter ReceiveDuration { get; private set; }
		public RuntimePerformanceCounter ReceiveDurationBase { get; private set; }
		public RuntimePerformanceCounter ReceiveRate { get; private set; }
		public RuntimePerformanceCounter ReceiveThreadCount { get; private set; }
		public RuntimePerformanceCounter SendRate { get; private set; }
		public RuntimePerformanceCounter SentCount { get; private set; }

		void InitiatizeCategory()
		{
			try
			{
				var counters = new[]
					{
						ConsumerThreadCount,
						ReceiveThreadCount,
						ReceiveRate,
						PublishRate,
						SendRate,
						ReceiveCount,
						PublishCount,
						SentCount,
						ConsumerDuration,
						ConsumerDurationBase,
						ReceiveDuration,
						ReceiveDurationBase,
						PublishDuration,
						PublishDurationBase,
					};

				if (!PerformanceCounterCategory.Exists(CategoryName))
				{
					PerformanceCounterCategory.Create(
						CategoryName,
						CategoryHelp,
						PerformanceCounterCategoryType.MultiInstance,
						new CounterCreationDataCollection(counters.Select(x => (CounterCreationData) x).ToArray()));

					return;
				}

				int missing = counters
					.Where(counter => !PerformanceCounterCategory.CounterExists(counter.Name, CategoryName))
					.Count();

				if (missing > 0)
				{
					PerformanceCounterCategory.Delete(CategoryName);

					PerformanceCounterCategory.Create(
						CategoryName,
						CategoryHelp,
						PerformanceCounterCategoryType.MultiInstance,
						new CounterCreationDataCollection(counters.Select(x => (CounterCreationData) x).ToArray()));
				}
			}
			catch (SecurityException ex)
			{
				_log.Error("Unable to create performance counter category (Category: {0})\nTry running the program in the Administrator role to set these up.".FormatWith(CategoryName), ex);
			}
		}
	}
}