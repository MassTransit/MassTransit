using System;
using Automatonymous.Activities;
using Automatonymous.Binders;
using MassTransit;

namespace Automatonymous
{
	public static partial class SendExtensions
	{
		public static EventActivityBinder<TInstance, TData> SendMany<TInstance, TData, TInput, TMessage>(
			this EventActivityBinder<TInstance, TData> source,
			InputFactory<TInstance, TData, TInput> inputFactory,
			DestinationAddressProvider<TInstance, TData, TInput> destinationAddressProvider,
			EventMessageFactory<TInstance, TData, TInput, TMessage> messageFactory)
			where TInstance : class, SagaStateMachineInstance
			where TMessage : class
			where TData : class
		{
			return source.Add(new SendManyActivity<TInstance, TData, TInput, TMessage>(inputFactory, destinationAddressProvider, messageFactory));
		}

		public static EventActivityBinder<TInstance, TData> SendMany<TInstance, TData, TInput, TMessage>(
			this EventActivityBinder<TInstance, TData> source,
			InputFactory<TInstance, TData, TInput> inputFactory,
			DestinationAddressProvider<TInstance, TData, TInput> destinationAddressProvider,
			EventMessageFactory<TInstance, TData, TInput, TMessage> messageFactory,
			Action<SendContext<TMessage>> contextCallback)
			where TInstance : class, SagaStateMachineInstance
			where TMessage : class
			where TData : class
		{
			return source.Add(new SendManyActivity<TInstance, TData, TInput, TMessage>(inputFactory, destinationAddressProvider, messageFactory, contextCallback));
		}
	}
}