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
namespace MassTransit.Subscriptions
{
	using System;
	using Pipeline.Inspectors;
	using Pipeline.Sinks;
	using Saga;


	/// <summary>
	/// Collects all the active message types from the pipeline
	/// </summary>
	public class SubscriptionCollector :
		PipelineInspectorBase
	{
		private readonly SubscriptionSnapshot _snapshot = new SubscriptionSnapshot();

		public SubscriptionSnapshot Snapshot
		{
			get { return _snapshot; }
		}

		private void Add(Type messageType)
		{
			_snapshot.Add(messageType);
		}

		private void Add(Type messageType, string correlationId)
		{
			_snapshot.Add(messageType, correlationId);
		}

		public bool Inspect<TMessage, TBatchId>(BatchMessageRouter<TMessage, TBatchId> sink)
			where TMessage : class, BatchedBy<TBatchId>
		{
			if(sink.SinkCount > 0)
				Add(typeof (TMessage));

			return true;
		}

		public bool Inspect<TMessage>(InstanceMessageSink<TMessage> sink) where TMessage : class
		{
			Add(typeof (TMessage));
			return true;
		}

		public bool Inspect<TMessage, TKey>(CorrelatedMessageSinkRouter<TMessage, TKey> sink)
			where TMessage : class, CorrelatedBy<TKey>
		{
			Add(typeof (TMessage), sink.CorrelationId.ToString());
			return true;
		}

		public bool Inspect<TComponent, TMessage>(ComponentMessageSink<TComponent, TMessage> sink)
			where TMessage : class
			where TComponent : class, Consumes<TMessage>.All
		{
			Add(typeof (TMessage));
			return true;
		}

		public bool Inspect<TComponent, TMessage>(InitiateSagaMessageSink<TComponent, TMessage> sink)
			where TMessage : class, CorrelatedBy<Guid>
			where TComponent : class, Orchestrates<TMessage>, ISaga
		{
			Add(typeof (TMessage));
			return true;
		}

		public bool Inspect<TComponent, TMessage>(OrchestrateSagaMessageSink<TComponent, TMessage> sink)
			where TMessage : class, CorrelatedBy<Guid>
			where TComponent : class, Orchestrates<TMessage>, ISaga
		{
			Add(typeof (TMessage));
			return true;
		}

		public bool Inspect<TComponent, TMessage>(SelectedComponentMessageSink<TComponent, TMessage> sink)
			where TMessage : class
			where TComponent : class, Consumes<TMessage>.Selected
		{
			Add(typeof (TMessage));
			return true;
		}
	}
}