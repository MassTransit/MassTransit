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
namespace MassTransit.Pipeline.Inspectors
{
	using System;
	using System.Linq;
	using System.Text;
	using Batch;
	using Batch.Pipeline;
	using Distributor.Pipeline;
	using Saga;
	using Saga.Pipeline;
	using Sinks;

	public class PipelineViewer :
		PipelineInspectorBase<PipelineViewer>
	{
		private readonly StringBuilder _text = new StringBuilder();
		private int _depth;

		public string Text
		{
			get { return _text.ToString(); }
		}

		protected override void IncreaseDepth()
		{
			_depth++;
		}

		protected override void DecreaseDepth()
		{
			_depth--;
		}

		public bool Inspect(MessagePipeline element)
		{
			Append("Pipeline");

			return true;
		}

		public bool Inspect<TMessage>(MessageRouter<TMessage> element) where TMessage : class
		{
			Append(string.Format("Routed ({0})", typeof (TMessage).ToFriendlyName()));

			return true;
		}

		public bool Inspect<TMessage, TBatchId>(BatchMessageRouter<TMessage, TBatchId> element)
			where TMessage : class, BatchedBy<TBatchId>
		{
			Append(string.Format("Batch Routed ({0})", typeof (TMessage).Name));

			return true;
		}

		public bool Inspect<TMessage, TBatchId>(BatchCombiner<TMessage, TBatchId> element)
			where TMessage : class, BatchedBy<TBatchId>
		{
			Append(string.Format("Batch Combiner ({0}) [{1}]", typeof (TMessage).Name, element.BatchId));

			return true;
		}

		public bool Inspect<TMessage>(MessageFilter<TMessage> element) where TMessage : class
		{
			Append(string.Format("Filtered '{0}' ({1})", element.Description, typeof (TMessage).Name));

			return true;
		}

		public bool Inspect(MessageInterceptor element)
		{
			Append(string.Format("Interceptor"));

			return true;
		}

		public bool Inspect<TMessage>(InstanceMessageSink<TMessage> sink) where TMessage : class
		{
			Append(string.Format("Consumed by Instance ({0})", typeof (TMessage).Name));

			return true;
		}

		public bool Inspect<TMessage>(DistributorMessageSink<TMessage> sink) where TMessage : class
		{
			Append(string.Format("Distributor ({0})", typeof (TMessage).Name));

			return true;
		}

		public bool Inspect<TSaga, TMessage>(SagaWorkerMessageSink<TSaga, TMessage> sink) where TMessage : class
			where TSaga : SagaStateMachine<TSaga>, ISaga
		{
			Append(string.Format("Saga Distributor Worker ({0} - {1})", typeof(TSaga).Name, typeof (TMessage).Name));

			return true;
		}

		public bool Inspect<TMessage>(WorkerMessageSink<TMessage> sink) where TMessage : class
		{
			Type messageType = typeof (TMessage).GetGenericArguments().First();

			Append(string.Format("Distributor Worker ({0})", messageType.Name));

			return true;
		}

		public bool Inspect<TMessage>(EndpointMessageSink<TMessage> sink) where TMessage : class
		{
			Append(string.Format("Send {0} to Endpoint {1}", typeof (TMessage).Name, sink.Address));

			return true;
		}

		public bool Inspect<TMessage>(IPipelineSink<TMessage> sink) where TMessage : class
		{
			Append(string.Format("Unknown Message Sink {0} ({1})", sink.GetType(), typeof (TMessage).Name));

			return true;
		}

		public bool Inspect<TMessage, TKey>(CorrelatedMessageRouter<TMessage, TKey> sink)
			where TMessage : class, CorrelatedBy<TKey>
		{
			Append(string.Format("Correlated by {1} ({0})", typeof (TMessage).Name, typeof (TKey).Name));

			return true;
		}

		public bool Inspect<TMessage, TKey>(CorrelatedMessageSinkRouter<TMessage, TKey> sink)
			where TMessage : class, CorrelatedBy<TKey>
		{
			Append(string.Format("Routed for Correlation Id {1} ({0})", typeof (TMessage).Name, sink.CorrelationId));

			return true;
		}


		public bool Inspect<TComponent, TMessage>(ComponentMessageSink<TComponent, TMessage> sink)
			where TMessage : class
			where TComponent : class, Consumes<TMessage>.All
		{
			Type componentType = typeof (TComponent);

			string componentName = componentType.IsGenericType ? componentType.GetGenericTypeDefinition().FullName : componentType.FullName;

			Append(string.Format("Consumed by Component {0} ({1})", componentName, typeof (TMessage).Name));

			return true;
		}

		public bool Inspect<TComponent, TMessage>(CorrelatedSagaMessageSink<TComponent, TMessage> sink)
			where TMessage : class, CorrelatedBy<Guid>
			where TComponent : class, Consumes<TMessage>.All, ISaga
		{
			Type componentType = typeof (TComponent);

			string componentName = componentType.IsGenericType ? componentType.GetGenericTypeDefinition().FullName : componentType.FullName;

			string policyDescription = GetPolicy(sink.Policy);

			Append(string.Format("{0} Saga {1} ({2})", policyDescription, componentName, typeof (TMessage).Name));

			return true;
		}

		private string GetPolicy<TComponent, TMessage>(ISagaPolicy<TComponent, TMessage> policy)
			where TComponent : class, ISaga
		{
			string description;
			Type policyType = policy.GetType().GetGenericTypeDefinition();
			if (policyType == typeof (InitiatingSagaPolicy<,>))
				description = "Initiates New";
			else if (policyType == typeof (ExistingSagaPolicy<,>))
				description = "Orchestrates Existing";
			else if (policyType == typeof (CreateOrUseExistingSagaPolicy<,>))
				description = "Initiates New Or Orchestrates Existing";
			else
				description = policyType.Name;
			return description;
		}

		public bool Inspect<TComponent, TMessage>(PropertySagaMessageSink<TComponent, TMessage> sink)
			where TMessage : class
			where TComponent : class, Consumes<TMessage>.All, ISaga
		{
			Type componentType = typeof (TComponent);

			string componentName = componentType.IsGenericType ? componentType.GetGenericTypeDefinition().FullName : componentType.FullName;

			string policyDescription = GetPolicy(sink.Policy);
			string expression = sink.Selector.ToString();

			Append(string.Format("{0} Saga {1} ({2}): {3}", policyDescription, componentName, typeof (TMessage).Name, expression));

			return true;
		}

		public bool Inspect<TComponent, TMessage>(PropertySagaStateMachineMessageSink<TComponent, TMessage> sink)
			where TMessage : class
			where TComponent : SagaStateMachine<TComponent>, ISaga
		{
			Type componentType = typeof (TComponent);

			string componentName = componentType.IsGenericType ? componentType.GetGenericTypeDefinition().FullName : componentType.FullName;

			string policyDescription = GetPolicy(sink.Policy);
			string expression = sink.Selector.ToString();

			Append(string.Format("{0} Saga {1} ({2}): {3}", policyDescription, componentName, typeof (TMessage).Name, expression));

			return true;
		}

		public bool Inspect<TComponent, TMessage>(CorrelatedSagaStateMachineMessageSink<TComponent, TMessage> sink)
			where TMessage : class, CorrelatedBy<Guid>
			where TComponent : SagaStateMachine<TComponent>, ISaga
		{
			Type componentType = typeof (TComponent);

			string componentName = componentType.IsGenericType ? componentType.GetGenericTypeDefinition().FullName : componentType.FullName;

			string policyDescription = GetPolicy(sink.Policy);

			Append(string.Format("{0} SagaStateMachine {1} ({2})", policyDescription, componentName, typeof (TMessage).Name));

			return true;
		}

		public bool Inspect<TComponent, TMessage>(SelectedComponentMessageSink<TComponent, TMessage> sink)
			where TMessage : class
			where TComponent : class, Consumes<TMessage>.Selected
		{
			Append(string.Format("Conditionally Consumed by Component {0} ({1})", typeof (TComponent).FullName, typeof (TMessage).Name));

			return true;
		}

		public bool Inspect<TInput, TOutput>(MessageTranslator<TInput, TOutput> translator) where TInput : class where TOutput : class, TInput
		{
			Append(string.Format("Translated from {0} to {1}", typeof (TInput).FullName, typeof (TOutput).FullName));

			return true;
		}

		private void Pad()
		{
			_text.Append(new string('\t', _depth));
		}

		private void Append(string text)
		{
			Pad();

			_text.AppendFormat(text).AppendLine();
		}

		public static void Trace(IMessagePipeline pipeline)
		{
			PipelineViewer viewer = new PipelineViewer();

			pipeline.Inspect(viewer);

			System.Diagnostics.Trace.WriteLine(viewer.Text);
		}
	}
}