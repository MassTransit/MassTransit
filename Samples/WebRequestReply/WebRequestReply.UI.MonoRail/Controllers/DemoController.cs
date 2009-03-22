namespace WebRequestReply.UI.MonoRail.Controllers
{
	using System;
	using Castle.MonoRail.Framework;
	using Core;
	using MassTransit;

	public class DemoController :
		SmartDispatcherController
	{
		private readonly IServiceBus _bus;

		public DemoController(IServiceBus bus)
		{
			_bus = bus;
		}

		public void Default()
		{
		}


		public void Sync(string requestText)
		{

		}

		//http://www.ayende.com/Blog/archive/2008/03/25/Async-Actions-in-Monorail.aspx
		public IAsyncResult BeginAsync(string requestText)
		{
			Guid requestId = Guid.NewGuid();

			return _bus.MakeRequest(x => x.Publish(new RequestMessage(requestId, requestText),
									y => y.SetResponseAddress(_bus.Endpoint.Uri)))
				.When<ResponseMessage>().RelatedTo(requestId).IsReceived(message =>
				{
					PropertyBag.Add("responseText", message.Text + " (and my response)");
				})
				.OnTimeout(() =>
				{
					PropertyBag.Add("responseText", "Async Task Timeout");
				})
				.TimeoutAfter(TimeSpan.FromSeconds(10))
				.BeginSend(ControllerContext.Async.Callback, ControllerContext.Async.State);
		}

		public void EndAsync()
		{
			IAsyncResult r = ControllerContext.Async.Result;

			RenderView("Default");
		}
	}
}