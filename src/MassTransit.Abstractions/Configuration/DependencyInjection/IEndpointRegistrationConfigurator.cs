namespace MassTransit
{
    using Courier.Contracts;


    public interface IEndpointRegistrationConfigurator
    {
        /// <summary>
        /// Set the endpoint name, overriding the default endpoint name formatter
        /// </summary>
        string Name { set; }

        /// <summary>
        /// True if the endpoint should be removed after the endpoint is stopped
        /// </summary>
        bool Temporary { set; }

        /// <summary>
        /// Only specify when required, use <see cref="ConcurrentMessageLimit" /> first and
        /// only specific a <see cref="PrefetchCount" /> when the default is not appropriate
        /// </summary>
        int? PrefetchCount { set; }

        /// <summary>
        /// The maximum number of concurrent messages processing at one time on the endpoint. Is
        /// used to configure the transport efficiently.
        /// </summary>
        int? ConcurrentMessageLimit { set; }

        /// <summary>
        /// Defaults to true, which connects topics/exchanges/etc. to the endpoint queue at the broker.
        /// If set to false, no broker topology is configured (automatically set to false for courier
        /// activities since <see cref="RoutingSlip" /> should never be published).
        /// </summary>
        bool ConfigureConsumeTopology { set; }

        /// <summary>
        /// Specifies an identifier that uniquely identifies the endpoint instance, which is appended to the
        /// end of the endpoint name.
        /// </summary>
        string InstanceId { set; }
    }
}
