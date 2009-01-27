namespace MassTransit.Configuration
{
	using System;
	using Internal;
	using Subscriptions;

	public class SubscriptionClientConfigurator :
		IServiceConfigurator
	{
		public void SetEndpoint(IEndpoint endpoint)
		{
			throw new NotImplementedException();
		}

		public void SetEndpoint(string uriString)
		{
			throw new NotImplementedException();
		}

		public void SetEndpoint(Uri uri)
		{
			throw new NotImplementedException();
		}

		public Type ServiceType
		{
			get { throw new System.NotImplementedException(); }
		}

		public IBusService Create(IServiceBus bus, ISubscriptionCache cache, IObjectBuilder builder)
		{
			throw new System.NotImplementedException();
		}
	}
}