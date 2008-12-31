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
	using System.Text;
	using System.Threading;
	using MassTransit.Grid;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;
	using Rhino.Mocks;

	[TestFixture]
	public class As_a_developer_that_needs_to_distribute_a_task_across_multiple_servers_for_parallel_processing :
		GridContextSpecification
	{
		private FactorLongNumbersTask _factorLongNumbers;
		private ManualResetEvent _complete;

		protected override void EstablishContext()
		{
			base.EstablishContext();

			_factorLongNumbers = new FactorLongNumbersTask();

			Random random = new Random();

			for (int i = 0; i < 27; i++)
			{
				long value = (long) (random.NextDouble()*1000000);

				_factorLongNumbers.Add(value);
			}

			_complete = new ManualResetEvent(false);

			_factorLongNumbers.WhenCompleted(x => _complete.Set());
		}

		[Test, Ignore]
		public void I_want_to_be_able_to_define_a_distributed_task_and_have_it_processed()
		{
			ObjectBuilder.Stub(x => x.GetInstance<FactorLongNumberWorker>()).Return(new FactorLongNumberWorker());

			LocalBus.Subscribe<SubTaskWorker<FactorLongNumberWorker, FactorLongNumber, LongNumberFactored>>();

			var distributedTaskController =
				new DistributedTaskController<FactorLongNumbersTask, FactorLongNumber, LongNumberFactored>(LocalBus, ObjectBuilder.GetInstance<IEndpointFactory>(), _factorLongNumbers);

			distributedTaskController.Start();

			Assert.That(_complete.WaitOne(TimeSpan.FromMinutes(1), true), Is.True, "Timeout waiting for distributed task to complete");
		}
	}

	public static class ListHelpers
	{
		public static string Join<T>(this IList<T> items, string separator)
		{
			StringBuilder sb = new StringBuilder();

			bool first = true;

			foreach (T item in items)
			{
				sb.Append(first ? item.ToString() : separator + item);

				first = false;
			}

			return sb.ToString();
		}
	}
}