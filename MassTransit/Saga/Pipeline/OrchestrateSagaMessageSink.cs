namespace MassTransit.Saga.Pipeline
{
	using System;
	using System.Collections.Generic;
	using Exceptions;
	using MassTransit.Pipeline.Interceptors;

	public class OrchestrateSagaMessageSink<TComponent, TMessage> :
		SagaMessageSinkBase<TComponent, TMessage>
		where TMessage : class, CorrelatedBy<Guid>
		where TComponent : class, Orchestrates<TMessage>, ISaga
	{
		public OrchestrateSagaMessageSink(IInterceptorContext context, IServiceBus bus, ISagaRepository<TComponent> repository)
			: base(context, bus, repository)
		{
		}

		public override IEnumerable<Consumes<TMessage>.All> Enumerate(TMessage message)
		{
			var sagaId = message.CorrelationId;
			if (sagaId == Guid.Empty)
				throw new MessageException(typeof (TMessage), "Orchestrated messages must have a valid correlation id");

			using (var enumerator = Repository.OrchestrateExistingSaga(sagaId))
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