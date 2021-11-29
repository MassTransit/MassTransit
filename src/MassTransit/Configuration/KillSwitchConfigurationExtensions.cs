namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using Configuration;
    using Transports.Components;


    public static class KillSwitchConfigurationExtensions
    {
        /// <summary>
        /// A Kill Switch monitors a receive endpoint and automatically stops and restarts the endpoint in the presence of consumer faults. The options
        /// can be configured to adjust the trip threshold, restart timeout, and exceptions that are observed by the kill switch. When configured on the bus,
        /// a kill switch is installed on every receive endpoint.
        /// </summary>
        /// <param name="configurator">The bus factory configurator</param>
        /// <param name="configure">Configure the kill switch options</param>
        public static void UseKillSwitch(this IBusFactoryConfigurator configurator, Action<KillSwitchOptions> configure = default)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var options = new KillSwitchOptions();

            configure?.Invoke(options);

            var observer = new EndpointConfigurationObserver(options);

            configurator.ConnectEndpointConfigurationObserver(observer);

            configurator.AddPipeSpecification(new KillSwitchOptionsSpecification(options));
        }

        /// <summary>
        /// A Kill Switch monitors a receive endpoint and automatically stops and restarts the endpoint in the presence of consumer faults. The options
        /// can be configured to adjust the trip threshold, restart timeout, and exceptions that are observed by the kill switch. When configured on a
        /// receive endpoint, a kill switch is installed on that receive endpoint only.
        /// </summary>
        /// <param name="configurator">The bus factory configurator</param>
        /// <param name="configure">Configure the kill switch options</param>
        public static void UseKillSwitch(this IReceiveEndpointConfigurator configurator, Action<KillSwitchOptions> configure = default)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var options = new KillSwitchOptions();

            configure?.Invoke(options);

            var observer = new KillSwitchReceiveEndpointObserver(options);

            configurator.ConnectReceiveEndpointObserver(observer);

            configurator.AddPipeSpecification(new KillSwitchOptionsSpecification(options));
        }


        class EndpointConfigurationObserver :
            IEndpointConfigurationObserver
        {
            readonly KillSwitchReceiveEndpointObserver _observer;

            public EndpointConfigurationObserver(KillSwitchOptions options)
            {
                _observer = new KillSwitchReceiveEndpointObserver(options);
            }

            public void EndpointConfigured<T>(T configurator)
                where T : IReceiveEndpointConfigurator
            {
                configurator.ConnectReceiveEndpointObserver(_observer);
            }
        }


        class KillSwitchOptionsSpecification :
            IPipeSpecification<ConsumeContext>
        {
            readonly KillSwitchOptions _options;

            public KillSwitchOptionsSpecification(KillSwitchOptions options)
            {
                _options = options;
            }

            public void Apply(IPipeBuilder<ConsumeContext> builder)
            {
            }

            public IEnumerable<ValidationResult> Validate()
            {
                return _options.Validate();
            }
        }
    }
}
