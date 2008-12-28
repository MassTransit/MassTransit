namespace WebRequestReply.Core
{
	using System;
	using MassTransit;

	public class RequestReplyController :
		Consumes<ResponseMessage>.For<Guid>,
		IDisposable
	{
		private readonly IServiceBus _serviceBus;
		private readonly IRequestReplyView _view;
		private ServiceBusRequest<RequestReplyController> _request;
		private Guid _requestId;

		public RequestReplyController(IRequestReplyView view, IServiceBus serviceBus)
		{
			_view = view;
			_serviceBus = serviceBus;
		}

		public void Consume(ResponseMessage message)
		{
			_view.ResponseText = message.Text;
			_request.Complete();
		}

		public Guid CorrelationId
		{
			get { return _requestId; }
		}

		public void Dispose()
		{
			_request.Dispose();
		}

		public void SendRequest()
		{
			// demonstrates a wait-for-it method of invoking a request

			_requestId = Guid.NewGuid();

			_request = _serviceBus.Request().From(this);

			_request.Send(new RequestMessage(_requestId, _view.RequestText));

			if (_request.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(10), true))
			{
				// we have happy time
			}
			else
			{
				_view.ResponseText = "Timeout waiting for response";
			}
		}

		public IAsyncResult BeginRequest(object sender, EventArgs e, AsyncCallback callback, object state)
		{
			_requestId = Guid.NewGuid();

			_request = _serviceBus.Request()
				.From(this)
				.WithCallback(callback, state);

			_request.Send(new RequestMessage(_requestId, _view.RequestText));

			return _request;
		}

		public void EndRequest(IAsyncResult ar)
		{
		}

		public void OnTimeout(IAsyncResult ar)
		{
			_view.ResponseText = "Async Task Timeout";

			_request.Cancel();
		}
	}
}