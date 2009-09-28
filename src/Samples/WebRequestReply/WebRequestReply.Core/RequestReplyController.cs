namespace WebRequestReply.Core
{
	using System;
	using MassTransit;

	public class RequestReplyController 
	{
		private readonly IServiceBus _serviceBus;
		private readonly IRequestReplyView _view;

		public RequestReplyController(IRequestReplyView view, IServiceBus serviceBus)
		{
			_view = view;
			_serviceBus = serviceBus;
		}


		public void SendRequest()
		{
			Guid requestId = Guid.NewGuid();

			_serviceBus.MakeRequest(x => x.Publish(new RequestMessage(requestId, _view.RequestText),
									y => y.SetResponseAddress(_serviceBus.Endpoint.Uri)))
				.When<ResponseMessage>().RelatedTo(requestId).IsReceived(message =>
				{
					_view.ResponseText = message.Text;
				})
				.OnTimeout(() =>
				{
					_view.ResponseText = "Async Task Timeout";
				})
				.TimeoutAfter(TimeSpan.FromSeconds(10))
				.Send();
		}

		public IAsyncResult BeginRequest(object sender, EventArgs e, AsyncCallback callback, object state)
		{
			Guid requestId = Guid.NewGuid();

			return _serviceBus.MakeRequest(x => x.Publish(new RequestMessage(requestId, _view.RequestText), 
									y => y.SetResponseAddress(_serviceBus.Endpoint.Uri)))
				.When<ResponseMessage>().RelatedTo(requestId).IsReceived(message =>
					{
						_view.ResponseText = message.Text;
					})
				.OnTimeout(() =>
					{
						_view.ResponseText = "Async Task Timeout";
					})
				.TimeoutAfter(TimeSpan.FromSeconds(10))
				.BeginSend(callback, state);
		}

		public void EndRequest(IAsyncResult ar)
		{
		}
	}
}