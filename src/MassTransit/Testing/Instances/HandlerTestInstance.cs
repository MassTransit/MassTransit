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
	using TestContexts;

	public class HandlerTestInstance<TMessage> :
		HandlerTest<TMessage>
		where TMessage : class
	{
		readonly IList<TestAction> _actions;
		readonly HandlerTestSubject<TMessage> _subject;
		readonly IBusTestContext _testContext;

		bool _disposed;

		public HandlerTestInstance(IBusTestContext testContext, IList<TestAction> actions)
		{
			_testContext = testContext;
			_actions = actions;

			_subject = new HandlerTestSubjectImpl<TMessage>();
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public IReceivedMessageList Skipped
		{
			get { return _testContext.Skipped; }
		}

		public void Execute()
		{
			_subject.Prepare(_testContext.Bus);

			_actions.Each(x => x.Act(_testContext.Bus));
		}

		public ISentMessageList Sent
		{
			get { return _testContext.Sent; }
		}

		public IReceivedMessageList Received
		{
			get { return _testContext.Received; }
		}

		public HandlerTestSubject<TMessage> Handler
		{
			get { return _subject; }
		}

		void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				_subject.Dispose();

				_testContext.Dispose();
			}

			_disposed = true;
		}

		~HandlerTestInstance()
		{
			Dispose(false);
		}
	}
}