namespace CodeCamp.Web
{
	using System;
	using System.Web;
	using Core;

	public class Global : HttpApplication
	{
		protected void Application_Start(object sender, EventArgs e)
		{
			DomainContext.Initialize();
		}

		protected void Application_End(object sender, EventArgs e)
		{
		}
	}
}