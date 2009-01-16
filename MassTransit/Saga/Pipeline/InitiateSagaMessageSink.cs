namespace MassTransit.Saga.Pipeline
{
	using System;
	using System.Collections.Generic;
	using MassTransit.Pipeline.Interceptors;
	using Util;

	public class InitiateSagaMessageSink<TComponent, TMessage> :
		SagaMessageSinkBase<TComponent, TMessage>
		where TMessage : class, CorrelatedBy<Guid>
		where TComponent : class, Orchestrates<TMessage>, ISaga
	{
		public InitiateSagaMessageSink(IInterceptorContext context, IServiceBus bus, ISagaRepository<TComponent> repository) :
			base(context, bus, repository)
		{
		}

		public override IEnumerable<Consumes<TMessage>.All> Enumerate(TMessage message)
		{
			var sagaId = message.CorrelationId;
			if (sagaId == Guid.Empty)
				sagaId = CombGuid.NewCombGuid();

			using (var enumerator = Repository.InitiateNewSaga(sagaId))
			{
				while (enumerator.MoveNext())
				{
					var component = enumerator.Current;

					component.Bus = Bus;

					yield return component;
				}
			}
		}
	}
}