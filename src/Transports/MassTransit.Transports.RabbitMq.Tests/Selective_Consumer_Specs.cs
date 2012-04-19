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
	using System.Threading;
	using BusConfigurators;
	using Logging;
	using Magnum.Extensions;
	using Magnum.TestFramework;
	using NUnit.Framework;

	public interface SometimesInterestingMsg
	{
		string Msg { get; }
	}

	class SelectiveConsumer : Consumes<SometimesInterestingMsg>.Selected
	{
		static ILog logger = Logger.Get(typeof(SelectiveConsumer));

		int _no;
		int _consumed;

		public int No
		{
			get { return _no; }
		}

		public int Consumed
		{
			get { return _consumed; }
		}

		public void Consume(SometimesInterestingMsg message)
		{
			logger.Info("Consuming");
			Interlocked.Increment(ref _consumed);
		}

		public bool Accept(SometimesInterestingMsg message)
		{
			var accept = Interlocked.Increment(ref _no)%2 != 0;
			logger.Info(string.Format("Accepting: {0}", accept));
			return accept;
		}
	}

	public class Selective_Consumer_Specs
		: Given_a_rabbitmq_bus
	{
		SelectiveConsumer cons;
		const int TotalMsgs = 3;

		protected override void ConfigureServiceBus(Uri uri, ServiceBusConfigurator configurator)
		{
			base.ConfigureServiceBus(uri, configurator);

			cons = new SelectiveConsumer();
			configurator.Subscribe(s => s.Consumer(() => cons));
		}

		[When]
		public void A_message_is_published()
		{
			for (var i = 0; i < TotalMsgs; i++)
			{
				LocalBus.Publish<SometimesInterestingMsg>(new
					{
						Msg = string.Format("Hooray #{0}", i)
					});
			}
		}

		[Then]
		public void should_only_have_received_same_number_as_sent()
		{
			cons.No.ShouldBeEqualTo(TotalMsgs);
		}

		[Then]
		public void should_have_consumed_twice()
		{
			if (!SpinWait.SpinUntil(() => cons.Consumed == 2, 8.Seconds()))
				Assert.Fail("should have consumed two messages, skipping the one in the middle!");
		}
	}
}