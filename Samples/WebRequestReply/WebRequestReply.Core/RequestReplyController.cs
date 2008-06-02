namespace WebRequestReply.Core
{
	using System;
	using MassTransit.ServiceBus;

	public class RequestReplyController :
		Consumes<ResponseMessage>.For<Guid>,
		IDisposable
	{
		private readonly IServiceBus _serviceBus;
		private readonly IRequestReplyView _view;
		private AsyncController _asyncController;
		private Guid _requestId;

		public RequestReplyController(IRequestReplyView view, IServiceBus serviceBus)
		{
			_view = view;
			_serviceBus = serviceBus;

			_view.RequestEntered += View_RequestEntered;
		}

		public void Consume(ResponseMessage message)
		{
			_view.ResponseText = message.Text;
			_asyncController.Complete();
		}

		public Guid CorrelationId
		{
			get { return _requestId; }
		}


		private void View_RequestEntered(object sender, EventArgs e)
		{
			SendRequest();
		}

		public void SendRequest()
		{
			IAsyncResult asyncResult = BeginRequest(null, null, null, null);

			if (asyncResult.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(10), true))
			{
				EndRequest(asyncResult);
			}
			else
			{
				_view.ResponseText = "Timeout waiting for response";
			}
		}


        public IAsyncResult BeginRequest(object sender, EventArgs e, AsyncCallback callback, object extraData)
        {
            _asyncController = new AsyncController(callback, extraData);

            _requestId = Guid.NewGuid();

            _serviceBus.Subscribe(this);

            RequestMessage request = new RequestMessage(_requestId, _view.RequestText);

            _serviceBus.Publish(request);

            return _asyncController;
        }


        public void EndRequest(IAsyncResult ar)
        {
            _serviceBus.Unsubscribe(this);
        }


        public void OnTimeout(IAsyncResult ar)
        {
            this._view.ResponseText = "Async Task Timeout";
        }

		public void Dispose()
		{
			_serviceBus.Unsubscribe(this);
		}
	}
}