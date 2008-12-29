namespace MassTransit.Pipeline.Configuration
{
	using System;
	using System.Collections.Generic;
	using Batch.Pipeline;
	using Interceptors;
	using Saga.Pipeline;
	using Sinks;

	public class MessagePipelineConfigurator :
		IConfigurePipeline,
		IDisposable
	{
		private readonly IObjectBuilder _builder;
		private readonly UnsubscribeAction _emptyToken = () => false;
		private volatile bool _disposed;

		protected InterceptorList<IPipelineInterceptor> _interceptors = new InterceptorList<IPipelineInterceptor>();
		private MessagePipeline _pipeline;
		private ISubscriptionEvent _subscriptionEvent;

		public MessagePipelineConfigurator(IObjectBuilder builder, ISubscriptionEvent subscriptionEvent)
		{
			_builder = builder;
			_subscriptionEvent = subscriptionEvent;

			MessageRouter<object> router = new MessageRouter<object>();

			_pipeline = new MessagePipeline(router, this);

			// interceptors are inserted at the front of the list, so do them from least to most specific
			_interceptors.Register(new ConsumesAllInterceptor());
			_interceptors.Register(new ConsumesSelectedInterceptor());
			_interceptors.Register(new ConsumesForInterceptor());
			_interceptors.Register(new BatchInterceptor());
			_interceptors.Register(new OrchestratesInterceptor());
			_interceptors.Register(new InitiatesInterceptor());
		}

		#region IConfigurePipeline Members

		public UnregisterAction Register(IPipelineInterceptor interceptor)
		{
			return _interceptors.Register(interceptor);
		}

		public UnsubscribeAction Subscribe<TComponent>()
			where TComponent : class
		{
			return Subscribe((context, interceptor) => interceptor.Subscribe<TComponent>(context));
		}

		public UnsubscribeAction Subscribe<TComponent>(TComponent instance)
			where TComponent : class
		{
			return Subscribe((context, interceptor) => interceptor.Subscribe(context, instance));
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion

		protected virtual void Dispose(bool disposing)
		{
			if (!disposing || _disposed) return;

			if (_interceptors != null)
				_interceptors.Dispose();

			_pipeline = null;
			_interceptors = null;

			_disposed = true;
		}

		~MessagePipelineConfigurator()
		{
			Dispose(false);
		}

		public V Configure<V>(Func<IConfigurePipeline, V> action)
		{
			V result = action(this);

			return result;
		}

		private UnsubscribeAction Subscribe(Func<IInterceptorContext, IPipelineInterceptor, IEnumerable<UnsubscribeAction>> subscriber)
		{
			var context = new InterceptorContext(_pipeline, _builder, _subscriptionEvent);

			UnsubscribeAction result = null;

			_interceptors.ForEach(interceptor =>
				{
					foreach (UnsubscribeAction token in subscriber(context, interceptor))
					{
						if (result == null)
							result = token;
						else
							result += token;
					}
				});

			return result ?? _emptyToken;
		}

		public static implicit operator MessagePipeline(MessagePipelineConfigurator configurator)
		{
			return configurator._pipeline;
		}

		public static MessagePipelineConfigurator CreateDefault(IObjectBuilder builder, ISubscriptionEvent subscriptionEvent)
		{
			return new MessagePipelineConfigurator(builder, subscriptionEvent);
		}
	}
}