namespace MassTransit.ServiceBus.Internal
{
	using System;
	using System.Threading;
	using Threading;

	public class ReceiveThread : ManagedThread
	{
		private readonly ServiceBus _bus;
		private readonly IEndpoint _endpoint;
		private readonly TimeSpan _readTimeout = TimeSpan.FromSeconds(10);

		public ReceiveThread(ServiceBus bus, IEndpoint endpoint)
		{
			_bus = bus;
			_endpoint = endpoint;
		}

		protected override void RunThread(object obj)
		{
			WaitHandle[] handles = new WaitHandle[] {Shutdown};

			int result;
			while ((result = WaitHandle.WaitAny(handles, 0, false)) != 0)
			{
				if (result == WaitHandle.WaitTimeout)
					continue;

				object message = _endpoint.Receive(_readTimeout, AcceptMessageCheck);

				if (message != null)
				{
					_bus.Dispatch(message, DispatchMode.Asynchronous);
				}
			}
		}

		private bool AcceptMessageCheck(object obj)
		{
			return _bus.Accept(obj);
		}
	}
}