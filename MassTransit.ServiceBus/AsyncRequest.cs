namespace MassTransit.ServiceBus
{
	using System;
	using Internal;
	using Util;

	public static class AsyncRequest
	{
		public static AsyncRequestBuilder<T> From<T>(T component) where T : class
		{
			return new AsyncRequestBuilder<T>(component);
		}
	}

	public class AsyncRequest<TComponent> :
		BaseAsyncResult,
		IAsyncRequest
		where TComponent : class
	{
		private readonly IServiceBus _bus;
		private readonly TComponent _component;

		public AsyncRequest(IServiceBus bus, TComponent component, AsyncCallback callback, object state)
			: base(callback, state)
		{
			_bus = bus;
			_component = component;
		}

		public override void Complete(bool synchronously)
		{
			base.Complete(synchronously);

			_bus.Unsubscribe(_component);
		}

		public override void Cancel()
		{
			_bus.Unsubscribe(_component);
		}

		public IAsyncRequest Send<T>(T message) where T : class
		{
			_bus.Subscribe(_component);

			_bus.Publish(message);

			return this;
		}
	}

	public class AsyncRequestBuilder<TComponent> where TComponent : class
	{
		internal IServiceBus _bus;
		internal AsyncCallback _callback;
		internal TComponent _component;
		internal object _state;

		public AsyncRequestBuilder(TComponent component)
		{
			_component = component;
		}

		public static implicit operator AsyncRequest<TComponent>(AsyncRequestBuilder<TComponent> builder)
		{
			Guard.Against.Null(builder._component, "Source object must be specified");
			Guard.Against.Null(builder._bus, "A service bus must be specified");

			return new AsyncRequest<TComponent>(builder._bus, builder._component, builder._callback, builder._state);
		}

		public AsyncRequestBuilder<TComponent> Via(IServiceBus bus)
		{
			_bus = bus;
			return this;
		}

		public AsyncRequestBuilder<TComponent> WithCallback(AsyncCallback callback, object state)
		{
			_callback = callback;
			_state = state;

			return this;
		}

		public AsyncRequest<TComponent> Send<T>(T message) where T : class
		{
			AsyncRequest<TComponent> request = this;

			request.Send(message);

			return request;
		}
	}
}