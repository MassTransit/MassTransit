namespace MassTransit.ServiceBus.Internal
{
	using System;
	using System.Threading;
	using log4net;
	using Threading;

	public class ReceiveThread : ManagedThread
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof(ReceiveThread));
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

			while ((WaitHandle.WaitAny(handles, 0, false)) != 0)
			{
				try
				{
					object message = _endpoint.Receive(_readTimeout, AcceptMessageCheck);

					if (message != null)
					{
						_bus.Dispatch(message, DispatchMode.Asynchronous);
					}
				}
				catch(Exception ex)
				{
					_log.Error("Exception in ReceiveThread:", ex);
				}
			}
		}

		private bool AcceptMessageCheck(object obj)
		{
			return _bus.Accept(obj);
		}
	}
}