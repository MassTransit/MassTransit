namespace MassTransit.SubscriptionConfigurators
{
	using System;
	using System.Collections.Generic;
	using Configurators;
	using SubscriptionBuilders;

	public class SubscriptionRouterBuilderConfiguratorImpl :
		SubscriptionRouterBuilderConfigurator
	{
		readonly Action<SubscriptionRouterBuilder> _configureCallback;

		public SubscriptionRouterBuilderConfiguratorImpl(Action<SubscriptionRouterBuilder> configureCallback)
		{
			_configureCallback = configureCallback;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (_configureCallback == null)
				yield return this.Failure("ConfigureCallback", "Callback cannot be null");
		}

		public SubscriptionRouterBuilder Configure(SubscriptionRouterBuilder builder)
		{
			_configureCallback(builder);

			return builder;
		}
	}
}