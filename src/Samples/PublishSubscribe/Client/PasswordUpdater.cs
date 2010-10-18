namespace Client
{
    using log4net;
    using MassTransit;
    using SecurityMessages;

    public class PasswordUpdater :
        Consumes<PasswordUpdateComplete>.All
    {
		private static readonly ILog _log = LogManager.GetLogger(typeof(PasswordUpdater));

		public void Consume(PasswordUpdateComplete message)
        {
			_log.InfoFormat("Global password update complete: ({0})", message.CorrelationId);
        }
    }
}