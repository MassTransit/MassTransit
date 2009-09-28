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
namespace MassTransit.Tests.Grid
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Threading;
	using Magnum;
	using Magnum.DateTimeExtensions;
	using MassTransit.Grid.Configuration;
	using MassTransit.Transports;
	using NUnit.Framework;
	using Rhino.Mocks;

	[TestFixture]
	public class ThreeNodeGridTestFixture :
		GridTestFixture<LoopbackEndpoint>
	{
		protected override void EstablishContext()
		{
			base.EstablishContext();

			GridNodes.Each(x => x.ObjectBuilder.Stub(b => b.GetInstance<SimpleGridService>()).Return(new SimpleGridService()));

			WaitForServiceToBeAvailable<SimpleGridCommand>(5.Seconds(), 1);
		}

		protected override void ConfigureGridA(IGridConfigurator grid)
		{
			grid.For<SimpleGridCommand>()
				.Use<SimpleGridService>();
		}

		protected override void ConfigureGridB(IGridConfigurator grid)
		{
			grid.For<SimpleGridCommand>()
				.Use<SimpleGridService>();

			grid.SetProposer();
		}

		protected override void ConfigureGridC(IGridConfigurator grid)
		{
			grid.For<SimpleGridCommand>()
				.Use<SimpleGridService>();
		}
	}

	[TestFixture]
	public class When_multiple_nodes_support_the_same_services :
		ThreeNodeGridTestFixture
	{
		[Test, Explicit]
		public void The_first_available_node_should_be_voted_on_by_the_participating_nodes()
		{
			Guid transactionId = CombGuid.Generate();

			var received = new AutoResetEvent(false);
			int responseCount = 0;
			var unsubscribeAction = LocalBus.Subscribe<SimpleGridResult>(message =>
				{
					Interlocked.Increment(ref responseCount);
					received.Set();
				});
			using (unsubscribeAction.Disposable())
			{
				Thread.Sleep(250);

				LocalBus.Publish(new SimpleGridCommand(transactionId), context => context.SendResponseTo(LocalBus.Endpoint));

				while (received.WaitOne(5.Seconds(), true))
				{
					Trace.WriteLine("Got Something");
				}
			}

			Assert.AreEqual(1, responseCount);
		}
	}

	[TestFixture]
	public class When_throwing_a_bunch_of_commands_at_the_grid :
		ThreeNodeGridTestFixture
	{
		private readonly List<Guid> _responseList = new List<Guid>();
		private readonly List<string> _sourceList = new List<string>();
		private readonly Dictionary<Guid, int> _responses = new Dictionary<Guid,int>();
		private readonly Dictionary<string, int> _sources = new Dictionary<string, int>();

		[Test, Explicit]
		public void Each_command_should_only_be_processed_one_time()
		{
			var received = new AutoResetEvent(false);
			var unsubscribeAction = LocalBus.Subscribe<SimpleGridResult>(message =>
				{
					lock(_responseList)
						_responseList.Add(message.CorrelationId);

					lock(_sourceList)
						_sourceList.Add(CurrentMessage.Headers.SourceAddress.ToString());

					received.Set();
				});

			using (unsubscribeAction.Disposable())
			{
				Thread.Sleep(250);

				for (int i = 0; i < 100; i++)
				{
					LocalBus.Publish(new SimpleGridCommand(CombGuid.Generate()), context => context.SendResponseTo(LocalBus.Endpoint));
				}

				while (received.WaitOne(5.Seconds(), true))
				{
					Trace.Write(".");
				}
				Trace.WriteLine("");
			}

			TabulateResults();

			Assert.AreEqual(100, _responses.Count);

			_responses.Values.Each(x => Assert.AreEqual(1, x, "Too many results received"));

			_sources.Each(x => Trace.WriteLine(x.Key + ": " + x.Value + " results"));
		}

		private void TabulateResults()
		{
			_responseList.Each(x =>
				{
					if (_responses.ContainsKey(x))
						_responses[x] = _responses[x] + 1;
					else
						_responses.Add(x, 1);
				});

			_sourceList.Each(x =>
				{
					if (_sources.ContainsKey(x))
						_sources[x] = _sources[x] + 1;
					else
						_sources.Add(x, 1);
					
				});
		}
	}
}