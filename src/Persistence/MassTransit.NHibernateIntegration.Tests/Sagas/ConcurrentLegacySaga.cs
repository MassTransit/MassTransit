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
	using System.Diagnostics;
	using System.Threading;
	using MassTransit.Saga;
	using log4net;

    public class ConcurrentLegacySaga :
		ISaga,
		InitiatedBy<StartConcurrentSaga>,
		Orchestrates<ContinueConcurrentSaga>
	{
		static readonly ILog _log = LogManager.GetLogger(typeof (ConcurrentLegacySaga));


		public ConcurrentLegacySaga(Guid correlationId)
		{
			CorrelationId = correlationId;

			Value = -1;
		}

		protected ConcurrentLegacySaga()
		{
			Value = -1;
		}

		public  string Name { get; set; }
		public  int Value { get; set; }

		public  void Consume(StartConcurrentSaga message)
		{
			Trace.WriteLine("Consuming " + message.GetType());
			Thread.Sleep(3000);
			Name = message.Name;
			Value = message.Value;
			Trace.WriteLine("Completed " + message.GetType());
		}

		public  Guid CorrelationId { get; set; }
		public  IServiceBus Bus { get; set; }

		public  void Consume(ContinueConcurrentSaga message)
		{
			Trace.WriteLine("Consuming " + message.GetType());
			Thread.Sleep(1000);

			if (Value == -1)
				throw new InvalidOperationException("Should not be a -1 dude!!");

			Value = message.Value;
			Trace.WriteLine("Completed " + message.GetType());
		}
	}

	public class ConcurrentLegacySagaMap :
		SagaClassMapping<ConcurrentLegacySaga>
	{
		public ConcurrentLegacySagaMap()
		{
			Property(x => x.Name, x => x.Length(40));
			Property(x => x.Value);
		}
	}
}