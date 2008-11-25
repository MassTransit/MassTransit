namespace Server
{
	using log4net;
	using MassTransit;
	using SecurityMessages;

	public class PasswordUpdateService :
		IHostedService,
		Consumes<RequestPasswordUpdate>.All
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (PasswordUpdateService));
		private readonly IServiceBus _serviceBus;

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
			_serviceBus.Subscribe(this);
		}

		public void Stop()
		{
			_serviceBus.Unsubscribe(this);
		}

		#endregion

		public void Consume(RequestPasswordUpdate message)
		{
			_log.InfoFormat("Received password update: {0} ({1})", message.NewPassword, message.CorrelationId);

			_serviceBus.Publish(new PasswordUpdateComplete(message.CorrelationId, 0));
		}
	}
}