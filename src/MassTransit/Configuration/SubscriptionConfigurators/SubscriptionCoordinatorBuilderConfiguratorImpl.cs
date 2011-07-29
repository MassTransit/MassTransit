namespace MassTransit.SubscriptionConfigurators
{
	using System;
	using System.Collections.Generic;
	using Configurators;
	using SubscriptionBuilders;

	public class SubscriptionCoordinatorBuilderConfiguratorImpl :
		SubscriptionCoordinatorBuilderConfigurator
	{
		readonly Action<SubscriptionCoordinatorBuilder> _configureCallback;

		public SubscriptionCoordinatorBuilderConfiguratorImpl(Action<SubscriptionCoordinatorBuilder> configureCallback)
		{
			_configureCallback = configureCallback;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (_configureCallback == null)
				yield return this.Failure("ConfigureCallback", "Callback cannot be null");
		}

		public SubscriptionCoordinatorBuilder Configure(SubscriptionCoordinatorBuilder builder)
		{
			_configureCallback(builder);

			return builder;
		}
	}
}