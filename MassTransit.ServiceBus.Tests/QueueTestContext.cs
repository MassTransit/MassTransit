namespace MassTransit.ServiceBus.Tests
{
	using System;
	using System.Diagnostics;
	using System.Reflection;
	using log4net;
	using MassTransit.ServiceBus.Subscriptions;
	using Rhino.Mocks;

	public class QueueTestContext :
		IDisposable
	{
		protected static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private MockRepository _mocks;

		private ServiceBus _remoteServiceBus;
		private IEndpoint _remoteServiceBusEndPoint;

		private ServiceBus _serviceBus;
		private IEndpoint _serviceBusEndPoint;
		private IEndpoint _subscriptionEndpoint;

		private ISubscriptionCache _subscriptionCache;

		public QueueTestContext(string remoteMachineName)
		{
			Initialize();

			SetupResult.For(_remoteServiceBusEndPoint.Uri).Return(new Uri("local://" + remoteMachineName + "/test_remoteservicebus"));
			_mocks.ReplayAll();
		}

		public QueueTestContext()
		{
			Initialize();

			SetupResult.For(_remoteServiceBusEndPoint.Uri).Return(new Uri("local://" + Environment.MachineName.ToLowerInvariant() + "/test_remoteservicebus"));
			_mocks.ReplayAll();
		}

		public IServiceBus ServiceBus
		{
			get { return _serviceBus; }
		}

		public IServiceBus RemoteServiceBus
		{
			get { return _remoteServiceBus; }
		}

		public IEndpoint RemoteServiceBusEndPoint
		{
			get { return _remoteServiceBusEndPoint; }
		}

		public IEndpoint ServiceBusEndPoint
		{
			get { return _serviceBusEndPoint; }
		}

		public IEndpoint SubscriptionEndpoint
		{
			get { return _subscriptionEndpoint; }
		}

		#region IDisposable Members

		public void Dispose()
		{
			_log.Info("QueueTestContext Disposing");

			if (_remoteServiceBus != null)
				_remoteServiceBus.Dispose();

			if (_serviceBus != null)
				_serviceBus.Dispose();
		}

		#endregion

		private void Initialize()
		{
			StackTrace stackTrace = new StackTrace();
			StackFrame stackFrame = stackTrace.GetFrame(1);
			MethodBase methodBase = stackFrame.GetMethod();

			_log.InfoFormat("QueueTestContext Created for {0}", methodBase.Name);

			_mocks = new MockRepository();

			_remoteServiceBusEndPoint = _mocks.DynamicMock<IEndpoint>();
			_subscriptionEndpoint = _mocks.DynamicMock<IEndpoint>();
			_serviceBusEndPoint = _mocks.DynamicMock<IEndpoint>();

			_subscriptionCache = new LocalSubscriptionCache();

			_serviceBus = new ServiceBus(ServiceBusEndPoint, _subscriptionCache);
			_remoteServiceBus = new ServiceBus(RemoteServiceBusEndPoint, _subscriptionCache);

			SetupResult.For(_subscriptionEndpoint.Uri).Return(new Uri("local://" + Environment.MachineName.ToLowerInvariant() + "/test_subscriptions"));
			SetupResult.For(_serviceBusEndPoint.Uri).Return(new Uri("local://" + Environment.MachineName.ToLowerInvariant() + "/test_servicebus"));
		}
	}
}