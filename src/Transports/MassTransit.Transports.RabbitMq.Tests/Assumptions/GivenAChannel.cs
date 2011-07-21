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
namespace MassTransit.Transports.RabbitMq.Tests.Assumptions
{
	using System;
	using System.Diagnostics;
	using Magnum.Extensions;
	using NUnit.Framework;
	using RabbitMQ.Client;

	public class GivenAChannel
	{
		protected readonly byte[] TheMessage = new byte[] {1, 2, 3};
		IConnection _connection;

		[SetUp]
		public void BuildUpAConnection()
		{
			var cf = new ConnectionFactory();
			cf.UserName = "guest";
			cf.Password = "guest";
			cf.Port = 5672;
			cf.VirtualHost = "/";
			cf.HostName = "localhost";

			_connection = cf.CreateConnection();
		}

		[TearDown]
		public void TearDown()
		{
			_connection.Close();
		}

		public void WithStopWatch(string name, Action action)
		{
			var sw = new Stopwatch();
			sw.Start();
			action();
			sw.Stop();
			Trace.WriteLine("'{0}' took '{1}' seconds".FormatWith(name, sw.Elapsed.TotalSeconds));
		}

		public void WithChannel(Action<IModel> action)
		{
			IModel model = null;
			try
			{
				model = _connection.CreateModel();
				action(model);
			}
			finally
			{
				if (model != null)
				{
					model.Dispose();
				}
			}
		}
	}
}