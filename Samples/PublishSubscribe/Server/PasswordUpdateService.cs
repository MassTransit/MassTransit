namespace Server
{
	using System;
	using log4net;
	using MassTransit;
	using SecurityMessages;

	public class PasswordUpdateService :
		IHostedService,
		Consumes<RequestPasswordUpdate>.All
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (PasswordUpdateService));
		private readonly IServiceBus _serviceBus;
		private UnsubscribeAction _unsubscribeToken;

		public PasswordUpdateService(IServiceBus serviceBus)
		{
			_serviceBus = serviceBus;
		}

		#region IMessageService Members

		public void Dispose()
		{
			_serviceBus.Dispose();
		}

		public void Start()
		{
			_unsubscribeToken = _serviceBus.Subscribe(this);
		}

		public void Stop()
		{
			_unsubscribeToken();
		}

		#endregion

		public void Consume(RequestPasswordUpdate message)
		{
			_log.InfoFormat("Received password update: {0} ({1})", message.NewPassword, message.CorrelationId);

			_serviceBus.Publish(new PasswordUpdateComplete(message.CorrelationId, 0));

            _log.InfoFormat("Published password update complete message");
		}
	}
}