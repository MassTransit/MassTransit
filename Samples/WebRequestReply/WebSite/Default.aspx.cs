using System;
using System.Web.UI;
using MassTransit.ServiceBus;
using MassTransit.ServiceBus.Subscriptions;
using WebRequestReply.Core;

public partial class _Default :
	Page, IRequestReplyView
{
	private readonly RequestReplyController _controller;
	private readonly IServiceBus _serviceBus;
	private readonly MessageQueueEndpoint _serviceEndpoint = @"msmq://localhost/test_servicebus";
	private readonly ISubscriptionStorage _subscriptionCache = new LocalSubscriptionCache();

	public _Default()
	{
		_serviceBus = new ServiceBus(_serviceEndpoint, _subscriptionCache);

		_controller = new RequestReplyController(this, _serviceBus, _serviceEndpoint);
	}

	#region IRequestReplyView Members

	public string RequestText
	{
		get { return requestText.Text; }
	}

	public string ResponseText
	{
		set { responseBox.Text = value; }
	}

	public event EventHandler RequestEntered;

	#endregion

	protected void Page_Load(object sender, EventArgs e)
	{
	}

	protected void Button1_Click(object sender, EventArgs e)
	{
		if (RequestEntered != null)
			RequestEntered(this, new EventArgs());
	}

	protected void requestText_TextChanged(object sender, EventArgs e)
	{
	}
}