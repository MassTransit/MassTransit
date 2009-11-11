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
namespace MassTransit.Transports.Msmq.Tests
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Threading;
	using Grid.Sagas;
	using Magnum;
	using Magnum.DateTimeExtensions;
	using MassTransit.Tests.Grid;
	using NUnit.Framework;

	[TestFixture, Category("Integration")]
	public class GridOnMsmq_Specs :
		ThreeNodeMsmqGridTestFixture
	{
		private readonly List<Guid> _responseList = new List<Guid>();
		private readonly List<string> _sourceList = new List<string>();
		private readonly Dictionary<Guid, int> _responses = new Dictionary<Guid, int>();
		private readonly Dictionary<string, int> _sources = new Dictionary<string, int>();
		private readonly List<double> _elapsed = new List<double>();
		private double _minResponseTime;
		private double _maxResponseTime;
		private double _meanResponseTime;

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

			_minResponseTime = double.MaxValue;
			_maxResponseTime = double.MinValue;
			_meanResponseTime = 0;
			_elapsed.Each(x =>
				{
					_minResponseTime = Math.Min(_minResponseTime, x);
					_maxResponseTime = Math.Max(_maxResponseTime, x);

					_meanResponseTime += x;
				});

			_meanResponseTime /= _responseList.Count;
		}

		[Test, Explicit]
		public void Each_command_should_only_be_processed_one_time()
		{
		    const int iterations = 200;
			var received = new AutoResetEvent(false);
			var unsubscribeAction = LocalBus.Subscribe<SimpleGridResult>(message =>
				{
					lock (_responseList)
						_responseList.Add(message.CorrelationId);

					lock (_sourceList)
						_sourceList.Add(CurrentMessage.Headers.SourceAddress.ToString());

					lock (_elapsed)
						_elapsed.Add((DateTime.UtcNow - message.CreatedAt).TotalMilliseconds);

					received.Set();
				});

			using (unsubscribeAction.Disposable())
			{
				Thread.Sleep(10000);

                for (int i = 0; i < iterations; i++)
				{
					LocalBus.Publish(new SimpleGridCommand(CombGuid.Generate()), context =>
						{
							context.SendResponseTo(LocalBus.Endpoint);
							context.IfNoSubscribers<SimpleGridCommand>(x =>
								{
									throw new InvalidOperationException("Subscribers were not ready at startup!");
								});
						});
					Thread.Sleep(100);
				}

				TimeSpan timeout = 60.Seconds();
				while (received.WaitOne(timeout, true))
				{
					Trace.Write(".");
					timeout = 10.Seconds();
				}
				Trace.WriteLine("");
			}

			TabulateResults();

			_sources.Each(x => Trace.WriteLine(x.Key + ": " + x.Value + " results"));


			GridNodes.Each(x =>
				{
					x.GridMessageNodeRepository
						.Where(command => command.CurrentState != GridMessageNode.Completed && command.CurrentState != GridMessageNode.ConsumeCompleted)
						.Each(command =>
							{
								Trace.WriteLine(x.DataBus.Endpoint.Uri + " Pending Command: " + command.CorrelationId);
							});
				});


			Assert.AreEqual(iterations, _responses.Count);

			_responses.Values.Each(x => Assert.AreEqual(1, x, "Too many results received"));

			Trace.WriteLine("Min Response Time: " + _minResponseTime.ToString("F4"));
			Trace.WriteLine("Max Response Time: " + _maxResponseTime.ToString("F4"));
			Trace.WriteLine("Mean Response Time: " + _meanResponseTime.ToString("F4"));

            Assert.AreEqual(iterations, _responses.Count);

            _responses.Values.Each(x => Assert.AreEqual(1, x, "Too many results received"));
		}
	}
}