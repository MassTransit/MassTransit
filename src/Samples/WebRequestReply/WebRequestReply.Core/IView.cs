namespace WebRequestReply.Core
{
	using System;

	public interface IView
	{
		bool IsPostBack { get; }
		bool IsValid { get; }

		event EventHandler Init;
		event EventHandler Load;

		void DataBind();
	}
}