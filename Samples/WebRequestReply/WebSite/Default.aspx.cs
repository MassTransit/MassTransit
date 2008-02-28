using System;
using System.Web.UI;
using MassTransit.ServiceBus;
using MassTransit.ServiceBus.MSMQ;
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

	~_Default()
	{
		_serviceBus.Dispose();
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

	protected void Button2_Click(object sender, EventArgs e)
	{
		RegisterAsyncTask(new PageAsyncTask(beginTask, endTask, timeoutTask, this));

	}

	private void timeoutTask(IAsyncResult ar)
	{
		ResponseText = "Async Task Timeout";
	}

	private void endTask(IAsyncResult ar)
	{
		_controller.EndRequest(ar);
	}

	private IAsyncResult beginTask(object sender, EventArgs e, AsyncCallback cb, object extraData)
	{
		return _controller.BeginRequest(cb, extraData);
	}
}