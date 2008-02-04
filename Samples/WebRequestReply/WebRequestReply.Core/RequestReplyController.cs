using System;
using MassTransit.ServiceBus;

namespace WebRequestReply.Core
{
	public class RequestReplyController
	{
		private readonly IRequestReplyView _view;
		private readonly IServiceBus _serviceBus;
		private readonly IEndpoint _serviceEndpoint;

		public RequestReplyController(IRequestReplyView view, IServiceBus serviceBus, IEndpoint serviceEndpoint)
		{
			_view = view;
			_serviceBus = serviceBus;
			_serviceEndpoint = serviceEndpoint;

			_view.RequestEntered += View_RequestEntered;

			_serviceBus.Subscribe<RequestMessage>(HandleRequestMessage);
		}

		private static void HandleRequestMessage(IMessageContext<RequestMessage> ctx)
		{
			ResponseMessage response = new ResponseMessage();
			response.Text = "Request: " + ctx.Message.Text;

			ctx.Reply(response);
		}

		void View_RequestEntered(object sender, System.EventArgs e)
		{
			SendRequest();
		}

		public void SendRequest()
		{
			IServiceBusAsyncResult asyncResult = BeginRequest(null, null);

			if(asyncResult.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(10), true))
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
			IServiceBusAsyncResult asyncResult = (IServiceBusAsyncResult)ar;

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