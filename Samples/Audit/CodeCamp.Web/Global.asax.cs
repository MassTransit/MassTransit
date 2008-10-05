namespace CodeCamp.Web
{
	using System;
	using System.Web;
	using Castle.MicroKernel;
	using Domain;
	using MassTransit.ServiceBus.Configuration;
	using MassTransit.WindsorIntegration;

    public class Global : HttpApplication
	{
		protected void Application_Start(object sender, EventArgs e)
		{
            BusBuilder.SetObjectBuilder(new WindsorObjectBuilder(new DefaultKernel()));
			DomainContext.Initialize();
		}

		protected void Application_End(object sender, EventArgs e)
		{
		}
	}
}