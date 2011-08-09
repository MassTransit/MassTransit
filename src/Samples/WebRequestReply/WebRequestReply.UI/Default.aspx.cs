namespace WebRequestReply.UI
{
	using System;
	using System.Web.UI;
	using Core;
	using MassTransit;

	public partial class _Default :
		Page, IRequestReplyView
	{
		readonly RequestReplyController _controller;

		public _Default()
		{
			IEndpoint targetService = Bus.Instance.GetEndpoint(new Uri(Global.ServiceUri));

			_controller = new RequestReplyController(this, Bus.Instance, targetService);
		}

		public string RequestText
		{
			get { return requestText.Text; }
		}

		public string ResponseText
		{
			set { responseBox.Text = value; }
		}

		protected void Button1_Click(object sender, EventArgs e)
		{
			_controller.SendRequest();
		}

		protected void Button2_Click(object sender, EventArgs e)
		{
			RegisterAsyncTask(new PageAsyncTask(_controller.BeginRequest, _controller.EndRequest, x => { }, this));
		}
	}
}