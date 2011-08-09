namespace WebRequestReply.Core
{
	using System;
	using Magnum;
	using Magnum.Extensions;
	using MassTransit;

	public class RequestReplyController
	{
		readonly IServiceBus _bus;
		readonly IEndpoint _service;
		readonly IRequestReplyView _view;

		public RequestReplyController(IRequestReplyView view, IServiceBus bus, IEndpoint service)
		{
			Guard.AgainstNull(view, "view");
			Guard.AgainstNull(bus, "bus");
			Guard.AgainstNull(service, "service");

			_view = view;
			_bus = bus;
			_service = service;
		}

		public void SendRequest()
		{
			Guid requestId = Guid.NewGuid();

			_service.SendRequest(new RequestMessage(requestId, _view.RequestText),
			                     _bus, rc =>
			                     	{
			                     		rc.HandleTimeout(8.Seconds(),
			                     		                 () => { _view.ResponseText = "Async Task Timeout"; });
			                     		rc.Handle<ResponseMessage>(rm => { _view.ResponseText = rm.Text; });
			                     	});
		}

		public IAsyncResult BeginRequest(object sender, EventArgs e, AsyncCallback callback, object state)
		{
			Guid requestId = Guid.NewGuid();

			return _service.BeginSendRequest(new RequestMessage(requestId, _view.RequestText),
			                                 _bus, callback, state, rc =>
			                                 	{
			                                 		rc.HandleTimeout(8.Seconds(),
			                                 		                 () => { _view.ResponseText = "Async Task Timeout"; });
			                                 		rc.Handle<ResponseMessage>(rm => { _view.ResponseText = rm.Text; });
			                                 	});
		}

		public void EndRequest(IAsyncResult ar)
		{
		}
	}
}