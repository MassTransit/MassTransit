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
namespace MassTransit.NHibernateIntegration.Tests.Sagas
{
	using System;
	using System.Threading;
	using FluentNHibernate.Mapping;
	using log4net;
	using Magnum.StateMachine;
	using MassTransit.Saga;

	public class ConcurrentSaga :
		SagaStateMachine<ConcurrentSaga>,
		ISaga
	{
		static readonly ILog _log = LogManager.GetLogger(typeof (ConcurrentSaga));

		static ConcurrentSaga()
		{
			Define(() =>
				{
					Initially(
						When(Start)
							.Then((saga, message) =>
								{
									_log.Info("Consuming " + message.GetType());
									Thread.Sleep(3000);
									saga.Name = message.Name;
									saga.Value = message.Value;
									_log.Info("Completed " + message.GetType());
								}).TransitionTo(Active));

					During(Active,
						When(Continue)
							.Then((saga, message) =>
								{
									_log.Info("Consuming " + message.GetType());
									Thread.Sleep(1000);
									saga.Value = message.Value;
									_log.Info("Completed " + message.GetType());
								}).Complete());
				});
		}

		public ConcurrentSaga(Guid correlationId)
		{
			CorrelationId = correlationId;
		}

		protected ConcurrentSaga()
		{
		}

		public static State Initial { get; set; }
		public static State Completed { get; set; }
		public static State Active { get; set; }

		public static Event<StartConcurrentSaga> Start { get; set; }
		public static Event<ContinueConcurrentSaga> Continue { get; set; }

		public virtual string Name { get; set; }
		public virtual int Value { get; set; }
		public virtual Guid CorrelationId { get; set; }
		public virtual IServiceBus Bus { get; set; }
	}

	[Serializable]
	public class ContinueConcurrentSaga :
		CorrelatedBy<Guid>
	{
		public int Value { get; set; }
		public virtual Guid CorrelationId { get; set; }
	}

	[Serializable]
	public class StartConcurrentSaga :
		CorrelatedBy<Guid>
	{
		public string Name { get; set; }

		public int Value { get; set; }
		public virtual Guid CorrelationId { get; set; }
	}

	public class ConcurrentSagaMap :
		ClassMap<ConcurrentSaga>
	{
		public ConcurrentSagaMap()
		{
			Id(x => x.CorrelationId)
				.GeneratedBy.Assigned();

			Map(x => x.CurrentState)
				.Access.ReadOnlyPropertyThroughCamelCaseField(Prefix.Underscore)
				.CustomType<StateMachineUserType>();


			Map(x => x.Name).Length(40);
			Map(x => x.Value);
		}
	}
}