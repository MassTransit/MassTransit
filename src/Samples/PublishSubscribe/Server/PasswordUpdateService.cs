namespace Server
{
	using log4net;
	using MassTransit;
	using SecurityMessages;

	public class PasswordUpdateService :
		Consumes<RequestPasswordUpdate>.All
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (PasswordUpdateService));
		private IServiceBus _serviceBus;
		private UnsubscribeAction _unsubscribeToken;

		public PasswordUpdateService()
		{
		}

		#region IMessageService Members

		public void Dispose()
		{
			_serviceBus.Dispose();
		}

		public void Start(IServiceBus bus)
		{
		    _serviceBus = bus;
			_unsubscribeToken = _serviceBus.SubscribeInstance(this);
		}

		public void Stop()
		{
			_unsubscribeToken();
		}

		#endregion

        //if the queue is transactional the transaction will already be started.
		public void Consume(RequestPasswordUpdate message)
		{
			_log.InfoFormat("Received password update: {0} ({1})", message.NewPassword, message.CorrelationId);

			_serviceBus.Publish(new PasswordUpdateComplete(message.CorrelationId, 0));

            _log.InfoFormat("Published password update complete message");
		}
	}
}