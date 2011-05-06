namespace HeavyLoad.Correlated
{
	using System;
	using Magnum.Extensions;
	using MassTransit;

	internal class CorrelatedController
	{
		static readonly TimeSpan _timeout = 12.Seconds();
		readonly IServiceBus _bus;
		readonly Guid _id;
		readonly Action<CorrelatedController> _successAction;
		readonly Action<CorrelatedController> _timeoutAction;

		public CorrelatedController(IServiceBus bus,
		                            Action<CorrelatedController> successAction,
		                            Action<CorrelatedController> timeoutAction)
		{
			_bus = bus;
			_successAction = successAction;
			_timeoutAction = timeoutAction;

			_id = Guid.NewGuid();
		}

		public void SimulateRequestResponse()
		{
			_bus.MakeRequest(x => x.Publish(new SimpleRequestMessage(_id), y => y.SetResponseAddress(_bus.Endpoint.Uri)))
				.When<SimpleResponseMessage>().RelatedTo(_id).IsReceived(message =>
					{
						if (message.CorrelationId != _id)
							throw new ArgumentException("Unknown message response received");

						// we got a response, that's a happy ending!
						_successAction(this);
					})
				.OnTimeout(() =>
					{
						// we timed out, not so happy
						_timeoutAction(this);
					})
				.TimeoutAfter(_timeout)
				.Send();
		}
	}
}