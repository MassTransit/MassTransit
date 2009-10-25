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
namespace MassTransit.Saga.Pipeline
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;
	using log4net;
	using Magnum.Reflection;
	using MassTransit.Pipeline;

	public abstract class SagaMessageSinkBase<TSaga, TMessage> :
		ISagaMessageSink<TSaga, TMessage>
		where TSaga : class,ISaga
		where TMessage : class
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (SagaMessageSinkBase<TSaga, TMessage>).ToFriendlyName());
		private bool _disposed;

		protected SagaMessageSinkBase(ISubscriberContext context,
		                              IServiceBus bus,
		                              ISagaRepository<TSaga> repository,
		                              ISagaPolicy<TSaga, TMessage> policy)
		{
			Repository = repository;
			Builder = context.Builder;
			Bus = bus;
			Policy = policy;

			SetBuilder = GetSetBuilderAction();
		}

		private Action<TSaga, IObjectBuilder> GetSetBuilderAction()
		{
			return typeof (TSaga)
				.GetProperties(BindingFlags.Public | BindingFlags.Instance)
				.Where(x => x.Name == "Builder")
				.Where(x => x.GetGetMethod(true) != null)
				.Where(x => x.GetSetMethod(true) != null)
				.Select(x => new FastProperty<TSaga, IObjectBuilder>(x).SetDelegate)
				.DefaultIfEmpty((x, y) => { })
				.SingleOrDefault();
		}

		public ISagaPolicy<TSaga, TMessage> Policy { get; private set; }
		public ISagaRepository<TSaga> Repository { get; private set; }
		public IObjectBuilder Builder { get; private set; }
		public IServiceBus Bus { get; private set; }
		public Action<TSaga, IObjectBuilder> SetBuilder { get; private set; }

		protected abstract Expression<Func<TSaga, TMessage, bool>> FilterExpression { get; }

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public IEnumerable<Action<TMessage>> Enumerate(TMessage itemxxx)
		{
			yield return message =>
				{
					var filter = CreateFilterExpressionForMessage(message);

					Repository.Send(filter, Policy, message, saga =>
						{
							saga.Bus = Bus;

							SetBuilder(saga, Builder);

							ConsumerAction(saga, message);
						});
				};
		}

		public bool Inspect(IPipelineInspector inspector)
		{
			return inspector.Inspect(this);
		}

		protected abstract void ConsumerAction(TSaga saga, TMessage message);

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
			}

			_disposed = true;
		}

		private Expression<Func<TSaga, bool>> CreateFilterExpressionForMessage(TMessage message)
		{
			return new SagaFilterExpressionConverter<TSaga, TMessage>(message).Convert(FilterExpression);
		}

		~SagaMessageSinkBase()
		{
			Dispose(false);
		}

		protected Expression<Func<TSaga, TMessage, bool>> CreateCorrelatedSelector()
		{
			var saga = Expression.Parameter(typeof (TSaga), "saga");
			var message = Expression.Parameter(typeof (TMessage), "message");

			var sagaId = Expression.Property(saga, typeof (TSaga).GetProperties().Where(x => x.Name == "CorrelationId").First());
			var messageId = Expression.Property(message, typeof (TMessage).GetProperties().Where(x => x.Name == "CorrelationId").First());

			var comparison = Expression.Equal(sagaId, messageId);

			return Expression.Lambda<Func<TSaga, TMessage, bool>>(comparison, new[] {saga, message});
		}
	}
}