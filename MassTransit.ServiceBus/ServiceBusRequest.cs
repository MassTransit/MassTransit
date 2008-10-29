/// Copyright 2007-2008 The Apache Software Foundation.
/// 
/// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
/// this file except in compliance with the License. You may obtain a copy of the 
/// License at 
/// 
///   http://www.apache.org/licenses/LICENSE-2.0 
/// 
/// Unless required by applicable law or agreed to in writing, software distributed 
/// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
/// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
/// specific language governing permissions and limitations under the License.
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
		private  IServiceBus _bus;
		private  AsyncCallback _callback;
		private  TComponent _component;
		private  object _state;
		private volatile bool _completedSynchronously;
		private volatile bool _isComplete = false;
		private bool _disposed;

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
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				if (_bus != null && _component != null)
					_bus.Unsubscribe(_component);

				_bus = null;
				_component = null;
				_callback = null;
				_state = null;
			}
			_disposed = true;
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

	/// <summary>
	/// The first step in building a request, this class is returned by IServiceBus.Request()
	/// and creates a specific RequestBuilder for the component making the request
	/// </summary>
	public class RequestBuilder
	{
		private readonly IServiceBus _bus;

		/// <summary>
		/// The service bus where the request originated
		/// </summary>
		/// <param name="bus"></param>
		public RequestBuilder(IServiceBus bus)
		{
			_bus = bus;
		}

		/// <summary>
		/// Associates a component with the request. Immediately before the request is sent
		/// the component is subscribed on the service bus. This registered any correlated
		/// handlers on the bus to receive messages sent in response to this request
		/// </summary>
		/// <typeparam name="T">The type of the component</typeparam>
		/// <param name="component">The component to associate with the request</param>
		/// <returns></returns>
		public RequestBuilder<T> From<T>(T component) where T : class
		{
			return new RequestBuilder<T>(component, _bus);
		}
	}

	/// <summary>
	/// A specialized request builder for the component making the request
	/// </summary>
	/// <typeparam name="TComponent">The type of component making the request</typeparam>
	public class RequestBuilder<TComponent> 
		where TComponent : class
	{
		private readonly IServiceBus _bus;
	    private AsyncCallback _callback;
	    private readonly TComponent _component;
	    private object _state;

		/// <summary>
		/// Creates a RequestBuilder object
		/// </summary>
		/// <param name="component"></param>
		/// <param name="bus">The service bus coordinating the request</param>
		public RequestBuilder(TComponent component, IServiceBus bus)
		{
			_component = component;
			_bus = bus;
		}

		/// <summary>
		/// Converts the builder to an instance of the ServiceBusRequest class.
		/// </summary>
		/// <param name="builder"></param>
		/// <returns></returns>
		public static implicit operator ServiceBusRequest<TComponent>(RequestBuilder<TComponent> builder)
		{
			Guard.Against.Null(builder._component, "Source object must be specified");
			Guard.Against.Null(builder._bus, "A service bus must be specified");

			return new ServiceBusRequest<TComponent>(builder._bus, builder._component, builder._callback, builder._state);
		}

		/// <summary>
		/// Specifies a callback to invoke when the component signals the request/response has completed.
		/// </summary>
		/// <param name="callback"></param>
		/// <param name="state"></param>
		/// <returns></returns>
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