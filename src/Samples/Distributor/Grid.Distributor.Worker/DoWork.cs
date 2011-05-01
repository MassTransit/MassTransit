namespace Grid.Distributor.Worker
{
	using System;
	using System.Configuration;
	using log4net;
	using MassTransit;
	using MassTransit.Distributor;
	using Shared;
	using Shared.Messages;

	public class DoWork :
		IServiceInterface,
		IDisposable,
		Consumes<DoSimpleWorkItem>.All
	{
		ILog _log = LogManager.GetLogger(typeof (DoWork));
		UnsubscribeAction _unsubscribeAction;

		public DoWork()
		{
			DataBus = ServiceBusFactory.New(x =>
				{
					x.ReceiveFrom(ConfigurationManager.AppSettings["SourceQueue"]);
			
					x.UseMsmq();
					x.UseMulticastSubscriptionClient();

					x.UseControlBus();
					x.SetConcurrentConsumerLimit(4);
					x.ImplementDistributorWorker<DoSimpleWorkItem>(ConsumeMessage, 2, 8);
				});

			ControlBus = DataBus.ControlBus;
		}

		public IServiceBus ControlBus { get; set; }
		public IServiceBus DataBus { get; set; }

		public void Consume(DoSimpleWorkItem message)
		{
			_log.InfoFormat("Responding to {0}", message.CorrelationId);
			CurrentMessage.Respond(new CompletedSimpleWorkItem(message.CorrelationId, message.CreatedAt));
		}

		public void Dispose()
		{
		}

		public void Start()
		{
			_unsubscribeAction = DataBus.SubscribeInstance(this);
		}

		public void Stop()
		{
			if (_unsubscribeAction != null)
				_unsubscribeAction();
		}

		public Action<DoSimpleWorkItem> ConsumeMessage(DoSimpleWorkItem message)
		{
			return m => { Consume(m); };
		}
	}
}