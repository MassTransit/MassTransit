namespace Client
{
    using System;
    using log4net;
    using MassTransit.ServiceBus;
    using SecurityMessages;

    public class AskPasswordQuestion :
        IHostedService,
		Consumes<PasswordUpdateComplete>.For<Guid>
    {
		private static readonly ILog _log = LogManager.GetLogger(typeof(AskPasswordQuestion));
		private readonly IServiceBus _bus;
    	private Guid _correlationId;

    	public AskPasswordQuestion(IServiceBus bus)
        {
            _bus = bus;
        }

    	public void Consume(PasswordUpdateComplete message)
    	{
			_log.InfoFormat("Correlated password update complete: {0} ({1})", message.ErrorCode, message.CorrelationId);
		}

    	public Guid CorrelationId
    	{
			get { return _correlationId; }
    	}

    	public void Start()
        {
            Console.WriteLine(new string('-', 20));
            Console.WriteLine("New Password Client");
            Console.WriteLine("What would you like to set your new password to?");
            Console.Write("New Password:");
            string newPassword = Console.ReadLine();

            Console.WriteLine(new string('-', 20));

    		RequestPasswordUpdate message = new RequestPasswordUpdate(newPassword);
    		_correlationId = message.CorrelationId;

    		_bus.Subscribe(this);

    		_bus.Publish(message);

            Console.WriteLine("Waiting For Reply");
            Console.WriteLine(new string('-', 20));
        }

        public void Stop()
        {
        	_bus.Unsubscribe(this);
        }

        public void Dispose()
        {
            
        }
    }
}