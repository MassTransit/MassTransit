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
	using System.Collections.Generic;
	using Scenarios;
	using Subjects;
	using TestActions;

	public class ConsumerTestInstance<TScenario, TConsumer> :
		TestInstance<TScenario>,
		ConsumerTest<TScenario, TConsumer>
		where TConsumer : class, IConsumer
	    where TScenario : TestScenario
	{
		readonly ConsumerTestSubjectImpl<TScenario, TConsumer> _subject;

		bool _disposed;

		public ConsumerTestInstance(TScenario scenario, IList<TestAction<TScenario>> actions,
		                            IConsumerFactory<TConsumer> consumerFactory)
			: base(scenario, actions)
		{
			_subject = new ConsumerTestSubjectImpl<TScenario, TConsumer>(consumerFactory);
		}

		public void Execute()
		{
			_subject.Prepare(Scenario);

			ExecuteTestActions();
		}

		public ConsumerTestSubject<TConsumer> Consumer
		{
			get { return _subject; }
		}

		protected override void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				_subject.Dispose();
			}

			base.Dispose(disposing);

			_disposed = true;
		}
	}
}