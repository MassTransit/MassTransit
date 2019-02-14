namespace MassTransit
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using Util;


    public static class SmartEndpointExtensions
    {
        public delegate void ReceiveEndpointConfiguration<out TEndpointConfigurator>(string queueName, Action<TEndpointConfigurator> configure)
            where TEndpointConfigurator : IReceiveEndpointConfigurator;


        public delegate void ReceiveEndpointConfiguration<in THost, out TEndpointConfigurator>(THost host, string queueName,
            Action<TEndpointConfigurator> configure)
            where THost : IHost
            where TEndpointConfigurator : IReceiveEndpointConfigurator;


        public static void SmartEndpoint<TConfigurator, TEndpointConfigurator>(this TConfigurator configurator,
            ReceiveEndpointConfiguration<TEndpointConfigurator> configuration, Action<TEndpointConfigurator> configure)
            where TConfigurator : IBusFactoryConfigurator
            where TEndpointConfigurator : IReceiveEndpointConfigurator
        {
        }

        public static void SmartEndpoint<TConfigurator, THost, TEndpointConfigurator>(this TConfigurator configurator, THost host,
            ReceiveEndpointConfiguration<THost, TEndpointConfigurator> configuration, Action<TEndpointConfigurator> configure)
            where TConfigurator : IBusFactoryConfigurator
            where THost : IHost
            where TEndpointConfigurator : IReceiveEndpointConfigurator
        {
        }

        public static void SmartEndpoint(this IInMemoryBusFactoryConfigurator configurator, Action<IInMemoryReceiveEndpointConfigurator> configure)
        {
            var partitioner = configurator.CreatePartitioner(Environment.ProcessorCount * 4);

            configurator.ReceiveEndpoint(new StewardEndpointDefinition(), null, cfg =>
            {
                //                cfg.UsePartitioner();
            });
        }

        static void ConfigureSmartEndpoint(IReceiveEndpointConfigurator configurator)
        {
        }
    }


    public class StewardEndpointDefinition :
        TemporaryEndpointDefinition
    {
        public StewardEndpointDefinition()
            : base("steward")
        {
        }
    }


    public class CheckServiceConsumer :
        IConsumer<CheckRequest>
    {
        public Task Consume(ConsumeContext<CheckRequest> context)
        {
            return TaskUtil.Completed;
        }
    }


    public enum QuotaCondition
    {
        BestEffort = 0,
        AllOrNothing = 1,
    }


    public interface Quota
    {
        /// <summary>
        /// The quota resource requested (if not specified, is related to the request itself)
        /// </summary>
        Uri Resource { get; }

        /// <summary>
        /// The number of requests requested, if making an allocation for rate purposes
        /// </summary>
        long? Quota { get; }

        /// <summary>
        /// The condition under which quota would be allocated
        /// </summary>
        QuotaCondition? Condition { get; }
    }


    /// <summary>
    /// Sent to check if a request is able to be processed by the endpoint
    /// </summary>
    public interface CheckRequest
    {
        /// <summary>
        /// The endpoint address, which is typically a queue endpoint
        /// </summary>
        Uri EndpointAddress { get; }

        /// <summary>
        /// The message type
        /// </summary>
        string MessageType { get; }

        /// <summary>
        /// If present, the quota associated with the request
        /// </summary>
        Quota[] Quotas { get; }
    }
}
