namespace Grid.Distributor.Worker
{
	using System;
	using System.Configuration;
	using System.Threading;
	using log4net;
	using MassTransit;
	using MassTransit.Distributor;
	using Shared;
	using Shared.Messages;

	public class DoWork :
		IServiceInterface,
		IDisposable
	{
		ILog _log = LogManager.GetLogger(typeof (DoWork));

		public DoWork()
		{
			DataBus = ServiceBusFactory.New(x =>
				{
					x.ReceiveFrom(ConfigurationManager.AppSettings["SourceQueue"]);
					x.SetPurgeOnStartup(true);

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

		public void Dispose()
		{
		}

		public void Start()
		{
			DataBus.InboundPipeline.View(Console.WriteLine);
		}

		public void Stop()
		{
		}

		Action<DoSimpleWorkItem> ConsumeMessage(DoSimpleWorkItem message)
		{
			return m =>
				{
					_log.InfoFormat("Responding to {0}", m.CorrelationId);
					DataBus.Context().Respond(new CompletedSimpleWorkItem(m.CorrelationId, m.CreatedAt));
				};
		}
	}
}