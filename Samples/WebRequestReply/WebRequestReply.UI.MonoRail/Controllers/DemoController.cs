namespace WebRequestReply.UI.MonoRail.Controllers
{
	using System;
	using Castle.MonoRail.Framework;
	using Core;
	using MassTransit.ServiceBus;

	public class DemoController :
		SmartDispatcherController,
		Consumes<ResponseMessage>.For<Guid>
	{
		private IServiceBus _bus;
		private Guid _correlationId;

		public DemoController(IServiceBus bus)
		{
			_bus = bus;
			_correlationId = Guid.NewGuid();
		}

		public void Default()
		{
		}


		public void Sync(string requestText)
		{
			RenderText("MT: " + requestText);
		}

		//http://www.ayende.com/Blog/archive/2008/03/25/Async-Actions-in-Monorail.aspx
		public IAsyncResult BeginAsync(string requestText)
		{
			_request = AsyncRequest.From(this)
				.Via(_bus)
				.WithCallback(ControllerContext.Async.Callback, ControllerContext.Async.State)
				.Send(new RequestMessage(CorrelationId, requestText));

			return _request;
		}

		public void EndAsync()
		{
			IAsyncResult r = ControllerContext.Async.Result;
			PropertyBag.Add("responseText", msg.Text + " (and my response)");
			RenderView("Default");
		}

		#region Consumes<ResponseMessage>.All Members

		private ResponseMessage msg;
		private IAsyncRequest _request;

		public void Consume(ResponseMessage message)
		{
			msg = message;
			_request.Complete();
		}

		#endregion

		#region CorrelatedBy<Guid> Members

		public Guid CorrelationId
		{
			get { return _correlationId; }
		}

		#endregion
	}
}