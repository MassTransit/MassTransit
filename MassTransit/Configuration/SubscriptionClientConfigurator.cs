namespace MassTransit.Configuration
{
	using System;

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
	}
}