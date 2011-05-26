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
namespace MassTransit.Testing.Instances
{
	using System;
	using System.Collections.Generic;
	using Configurators;
	using Magnum.Extensions;
	using Subjects;
	using TestActions;
	using TestContexts;

	public abstract class BusTestInstance
	{
		readonly IList<TestAction> _actions;
		readonly BusTestContext _testContext;
		bool _disposed;

		protected BusTestInstance(BusTestContext testContext, IList<TestAction> actions)
		{
			_testContext = testContext;
			_actions = actions;
		}

		public ReceivedMessageList Received
		{
			get { return _testContext.Received; }
		}

		public SentMessageList Sent
		{
			get { return _testContext.Sent; }
		}

		public ReceivedMessageList Skipped
		{
			get { return _testContext.Skipped; }
		}

		public BusTestContext TestContext
		{
			get { return _testContext; }
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				_testContext.Dispose();
			}

			_disposed = true;
		}

		protected void ExecuteTestActions()
		{
			_actions.Each(x => x.Act(_testContext.Bus));
		}

		~BusTestInstance()
		{
			Dispose(false);
		}
	}
}