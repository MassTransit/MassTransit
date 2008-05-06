using System;
using System.Web.UI;
using WebRequestReply.Core;

public partial class _Default :
	Page, IRequestReplyView
{
	private readonly RequestReplyController _controller;

	public _Default()
	{
		_controller = new RequestReplyController(this, Container.Instance.ServiceBus, Container.Instance.ServiceBus.Endpoint);
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

	public event EventHandler RequestEntered = delegate { };

	#endregion

	protected void Page_Load(object sender, EventArgs e)
	{
	}

	protected void Button1_Click(object sender, EventArgs e)
	{
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