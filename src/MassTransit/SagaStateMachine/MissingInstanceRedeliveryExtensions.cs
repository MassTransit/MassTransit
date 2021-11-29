namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using Configuration;


    public static class MissingInstanceRedeliveryExtensions
    {
        /// <summary>
        /// Redeliver uses the message scheduler to deliver the message to the queue at a future
        /// time. The delivery count is incremented.
        /// A message scheduler must be configured on the bus for redelivery to be enabled.
        /// </summary>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <param name="configurator">The consume context of the message</param>
        /// <param name="configure">Configure the retry policy for the message redelivery</param>
        /// <returns></returns>
        public static IPipe<ConsumeContext<TData>> Redeliver<TInstance, TData>(this IMissingInstanceConfigurator<TInstance, TData> configurator,
            Action<IMissingInstanceRedeliveryConfigurator<TInstance, TData>> configure)
            where TInstance : SagaStateMachineInstance
            where TData : class
        {
            var specification = new MissingInstanceRedeliveryConfigurator<TInstance, TData>(configurator);

            configure?.Invoke(specification);

            IReadOnlyList<ValidationResult> result = specification.Validate().ThrowIfContainsFailure();

            try
            {
                return specification.Build();
            }
            catch (Exception ex)
            {
                throw new ConfigurationException(result, "The missing instance redelivery configuration was invalid", ex);
            }
        }
    }
}
