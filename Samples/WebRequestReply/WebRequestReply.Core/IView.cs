using System;

namespace WebRequestReply.Core
{
	public interface IView
	{
		bool IsPostBack { get; }
		bool IsValid { get; }

		event EventHandler Init;
		event EventHandler Load;

		void DataBind();
	}
}