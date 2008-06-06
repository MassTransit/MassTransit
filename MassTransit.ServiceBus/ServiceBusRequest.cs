namespace MassTransit.ServiceBus
{
	using System;
	using System.Threading;
	using Util;

	public class ServiceBusRequest<TComponent> :
		IServiceBusRequest
		where TComponent : class
	{
		private readonly ManualResetEvent _asyncWaitHandle;
		private readonly IServiceBus _bus;
		private readonly AsyncCallback _callback;
		private readonly TComponent _component;
		private readonly object _state;
		private volatile bool _completedSynchronously;
		private volatile bool _isComplete = false;

		public ServiceBusRequest(IServiceBus bus, TComponent component, AsyncCallback callback, object state)
		{
			_bus = bus;
			_component = component;
			_callback = callback;
			_state = state;

			_asyncWaitHandle = new ManualResetEvent(false);
		}

		public bool IsCompleted
		{
			get { return _isComplete; }
		}

		public WaitHandle AsyncWaitHandle
		{
			get { return _asyncWaitHandle; }
		}

		public object AsyncState
		{
			get { return _state; }
		}

		public bool CompletedSynchronously
		{
			get { return _completedSynchronously; }
		}

		public void Cancel()
		{
			_asyncWaitHandle.Set();

			_bus.Unsubscribe(_component);
		}

		public void Complete()
		{
			Complete(false);
		}

		public void Dispose()
		{
			if (_bus != null && _component != null)
				_bus.Unsubscribe(_component);
		}

		public void Complete(bool synchronously)
		{
			_isComplete = true;
			_completedSynchronously = synchronously;
			_asyncWaitHandle.Set();

			if (_callback != null)
				_callback(this);

			_bus.Unsubscribe(_component);
		}

		public IServiceBusRequest Send<T>(T message) where T : class
		{
			_bus.Subscribe(_component);

			_bus.Publish(message);

			return this;
		}

		public IServiceBusRequest Send<T>(T message, RequestMode mode) where T : class
		{
			return Send(message, mode, TimeSpan.MaxValue);
		}

		public IServiceBusRequest Send<T>(T message, RequestMode mode, TimeSpan waitTimeout) where T : class
		{
			Send(message);

			if (mode == RequestMode.Synchronous)
			{
				AsyncWaitHandle.WaitOne(waitTimeout, true);

				if(!IsCompleted)
					Cancel();
			}

			return this;
		}
	}

	public class RequestBuilder
	{
		private readonly IServiceBus _bus;

		public RequestBuilder(IServiceBus bus)
		{
			_bus = bus;
		}

		public RequestBuilder<T> From<T>(T component) where T : class
		{
			return new RequestBuilder<T>(component, _bus);
		}
	}

	public class RequestBuilder<TComponent> where TComponent : class
	{
		internal IServiceBus _bus;
		internal AsyncCallback _callback;
		internal TComponent _component;
		internal object _state;

		public RequestBuilder(TComponent component)
		{
			_component = component;
		}

		public RequestBuilder(TComponent component, IServiceBus bus)
		{
			_component = component;
			_bus = bus;
		}

		public static implicit operator ServiceBusRequest<TComponent>(RequestBuilder<TComponent> builder)
		{
			Guard.Against.Null(builder._component, "Source object must be specified");
			Guard.Against.Null(builder._bus, "A service bus must be specified");

			return new ServiceBusRequest<TComponent>(builder._bus, builder._component, builder._callback, builder._state);
		}

		public RequestBuilder<TComponent> WithCallback(AsyncCallback callback, object state)
		{
			_callback = callback;
			_state = state;

			return this;
		}
	}

	public enum RequestMode
	{
		Synchronous,
		Asynchronous,
	}
}