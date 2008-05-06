namespace WebRequestReply.Core
{
	using System;
	using MassTransit.ServiceBus;

	public class RequestReplyController
	{
		private readonly IServiceBus _serviceBus;
		private readonly IEndpoint _serviceEndpoint;
		private readonly IRequestReplyView _view;

		public RequestReplyController(IRequestReplyView view, IServiceBus serviceBus, IEndpoint serviceEndpoint)
		{
			_view = view;
			_serviceBus = serviceBus;
			_serviceEndpoint = serviceEndpoint;

			_view.RequestEntered += View_RequestEntered;
		}


		private void View_RequestEntered(object sender, EventArgs e)
		{
			SendRequest();
		}

		public void SendRequest()
		{
			IServiceBusAsyncResult asyncResult = BeginRequest(null, null);

			if (asyncResult.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(10), true))
			{
				EndRequest(asyncResult);
			}
			else
			{
				_view.ResponseText = "Timeout waiting for response";
			}
		}

		public void EndRequest(IAsyncResult ar)
		{
			IServiceBusAsyncResult asyncResult = (IServiceBusAsyncResult) ar;

			ResponseMessage rm = asyncResult.Messages[0] as ResponseMessage;
			if (rm != null)
			{
				_view.ResponseText = rm.Text;
			}
			else
			{
				_view.ResponseText = "Invalid message type received";
			}
		}

		public IServiceBusAsyncResult BeginRequest(AsyncCallback callback, object data)
		{
			RequestMessage request = new RequestMessage();
			request.Text = _view.RequestText;

			IServiceBusAsyncResult asyncResult = _serviceBus.Request(_serviceEndpoint, callback, data, request);

			return asyncResult;
		}
	}
}