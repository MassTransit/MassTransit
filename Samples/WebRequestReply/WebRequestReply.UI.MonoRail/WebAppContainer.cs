namespace WebRequestReply.UI.MonoRail
{
	using Castle.Core.Resource;
	using Castle.Facilities.Startable;
	using Castle.MicroKernel.Registration;
	using Castle.MonoRail.Framework;
	using Castle.MonoRail.WindsorExtension;
	using Castle.Windsor.Configuration.Interpreters;
	using Controllers;
	using MassTransit.WindsorIntegration;

	public class WebAppContainer :
		DefaultMassTransitContainer
	{
		public WebAppContainer()
			: base(new XmlInterpreter(new ConfigResource()))
		{
			RegisterFacilities();
			RegisterComponents();
		}

		protected void RegisterFacilities()
		{
			AddFacility("rails", new MonoRailFacility());
			AddFacility("startable", new StartableFacility());
		}

		protected void RegisterComponents()
		{
			//new castle config!!
			//http://hammett.castleproject.org/?p=286
			Register(AllTypes.Of<SmartDispatcherController>()
			         	.FromAssembly(typeof (DemoController).Assembly));
		}
	}
}