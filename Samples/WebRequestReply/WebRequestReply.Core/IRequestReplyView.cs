using System;

namespace WebRequestReply.Core
{
	public interface IRequestReplyView :
		IView
	{
		string RequestText { get; }

		string ResponseText { set; }

		event EventHandler RequestEntered;
	}
}