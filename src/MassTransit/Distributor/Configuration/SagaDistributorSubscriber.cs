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
namespace MassTransit.Distributor.Configuration
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Magnum;
	using Magnum.Activator;
	using Magnum.Reflection;
	using Magnum.StateMachine;
	using MassTransit.Pipeline;
	using MassTransit.Pipeline.Configuration;
	using MassTransit.Pipeline.Configuration.Subscribers;
	using Messages;
	using Pipeline;
	using Saga;
	using Saga.Configuration;

	public class SagaDistributorSubscriber :
		PipelineSubscriberBase
	{
		private readonly ISagaPolicyFactory _policyFactory;

		public SagaDistributorSubscriber()
		{
			_policyFactory = new SagaPolicyFactory();
		}

		public override IEnumerable<UnsubscribeAction> Subscribe<TComponent>(ISubscriberContext context, TComponent instance)
		{
			Type distributorInterface = typeof (TComponent).GetInterfaces()
				.Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof (ISagaWorker<>))
				.SingleOrDefault();

			if (distributorInterface != null)
			{
				Type sagaType = distributorInterface.GetGenericArguments().First();

				var argumentTypes = new[] {typeof (TComponent), sagaType};
				var results = this.Call<IEnumerable<UnsubscribeAction>>("ConnectSinks", argumentTypes, context, instance);
				foreach (UnsubscribeAction result in results)
				{
					yield return result;
				}
			}

			yield break;
		}

		public override IEnumerable<UnsubscribeAction> Subscribe<TComponent>(ISubscriberContext context)
		{
			yield break;
		}

		private IEnumerable<UnsubscribeAction> ConnectSinks<TComponent, TSaga>(ISubscriberContext context, TComponent instance)
			where TComponent : ISagaWorker<TSaga>
			where TSaga : SagaStateMachine<TSaga>, ISaga
		{
			var saga = FastActivator<TSaga>.Create(CombGuid.Generate());

			var inspector = new SagaStateMachineEventInspector<TSaga>();
			saga.Inspect(inspector);

			foreach (var result in inspector.GetResults())
			{
				Type distributedType = typeof (Distributed<>).MakeGenericType(result.SagaEvent.MessageType);
				if (!context.HasMessageTypeBeenDefined(distributedType))
				{
					context.MessageTypeWasDefined(distributedType);

					yield return ConnectSink(context, instance, result.SagaEvent.MessageType, result.SagaEvent.Event, result.States);
				}
			}
		}

		private UnsubscribeAction ConnectSink<TSaga>(ISubscriberContext context, ISagaWorker<TSaga> worker, Type messageType, Event @event, IEnumerable<State> states)
		{
			return this.Call<UnsubscribeAction>("ConnectToSink", new[] {typeof(TSaga), messageType}, context, worker, @event, states);
		}

		private UnsubscribeAction ConnectToSink<TSaga, TMessage>(ISubscriberContext context, ISagaWorker<TSaga> worker, DataEvent<TSaga, TMessage> eevent, IEnumerable<State> states)
			where TSaga : SagaStateMachine<TSaga>, ISaga
			where TMessage : class
		{
			var factory = new SagaStateMachineMessageSinkFactory<TSaga, TMessage>(context, _policyFactory);
			IPipelineSink<TMessage> sink = factory.Create(eevent, states);

			var workerSink = new SagaWorkerMessageSink<TSaga, TMessage>(worker, sink);

			return context.Pipeline.ConnectToRouter(workerSink, () => context.SubscribedTo<Distributed<TMessage>>());
		}
	}
}