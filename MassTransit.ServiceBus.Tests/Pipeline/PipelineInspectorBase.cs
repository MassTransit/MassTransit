namespace MassTransit.ServiceBus.Tests
{
	using Pipeline;

	public class PipelineInspectorBase<T> : 
		IPipelineInspector 
		where T : class
	{

		public virtual bool Inspect<TMessage>(MessagePipeline<TMessage> element) where TMessage : class
		{
			return true;
		}

		public virtual bool Inspect<TMessage>(MessageRouter<TMessage> element) where TMessage : class
		{
			return true;
		}

		public virtual bool Inspect<TMessage>(MessageSink<TMessage> sink) where TMessage : class
		{
			return true;
		}

		public virtual bool Inspect<TInput, TOutput>(MessageTranslator<TInput, TOutput> translator)
			where TInput : class
			where TOutput : class, TInput
		{
			return true;
		}

		public virtual bool Inspect<TMessage>(IMessageSink<TMessage> element) where TMessage : class
		{
			return true;
		}
	}
}