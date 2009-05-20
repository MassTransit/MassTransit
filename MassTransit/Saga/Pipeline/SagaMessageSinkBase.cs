namespace MassTransit.Saga.Pipeline
{
	using System;
	using System.Collections.Generic;
	using System.Linq.Expressions;
	using Exceptions;
	using log4net;
	using MassTransit.Pipeline;
	using Util;

	public abstract class SagaMessageSinkBase<TSaga, TMessage> :
		ISagaMessageSink<TSaga, TMessage>
		where TSaga : ISaga
		where TMessage : class
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (SagaMessageSinkBase<TSaga, TMessage>).ToFriendlyName());

		private volatile bool _disposed;

		protected SagaMessageSinkBase(ISubscriberContext context,
		                              IServiceBus bus,
		                              ISagaRepository<TSaga> repository,
		                              ISagaPolicy<TSaga, TMessage> policy)
		{
			Repository = repository;
			Builder = context.Builder;
			Bus = bus;
			Policy = policy;
		}

		public ISagaPolicy<TSaga, TMessage> Policy { get; private set; }
		public ISagaRepository<TSaga> Repository { get; private set; }
		public IObjectBuilder Builder { get; private set; }
		public IServiceBus Bus { get; private set; }

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public IEnumerable<Action<TMessage>> Enumerate(TMessage item)
		{
			int count = 0;

			var filter = CreateFilterExpressionForMessage(item);

			foreach (var consumer in Repository.Find<TMessage>(filter, ConsumerAction))
			{
				if (!Policy.SagaShouldExist)
					throw new SagaException("The saga should not exist", typeof (TSaga), typeof (TMessage), filter);

				yield return consumer;

				count++;
			}

			if (count > 0)
				yield break;

			Guid sagaId;
			if (Policy.CreateSagaWhenMissing(item, out sagaId))
			{
				if (_log.IsDebugEnabled)
					_log.DebugFormat("Created saga [{0}] - {1}", typeof (TSaga).ToFriendlyName(), sagaId);

				foreach (var consumer in Repository.Create<TMessage>(sagaId, ConsumerAction))
				{
					yield return consumer;

					count++;
				}
			}
		}

		public bool Inspect(IPipelineInspector inspector)
		{
			return inspector.Inspect(this);
		}

		protected abstract Expression<Func<TSaga, TMessage, bool>> FilterExpression { get; }

		protected abstract void ConsumerAction(TSaga saga, TMessage message);

		private Expression<Func<TSaga, bool>> CreateFilterExpressionForMessage(TMessage message)
		{
			return new SagaFilterExpressionConverter<TSaga,TMessage>(message).Convert(FilterExpression);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
			}

			_disposed = true;
		}

		~SagaMessageSinkBase()
		{
			Dispose(false);
		}
	}
}