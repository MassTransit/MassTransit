using System.Reflection;
using System.Text.RegularExpressions;
using Confluent.Kafka;
using MassTransit;


namespace Dynamic.KafkaIntegration.Producer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;


    public static class RiderProducerExtensions
{
    // Name of IKafkaProducerConfiguration Configure method
    private const string KafkaProducerConfigurationMethod = "Configure";

    /// <summary>
    /// Add producer dynamically by inherit from IKafkaProducer interface
    /// if producer has any config you should inherit from IKafkaProducerConfiguration
    /// </summary>
    /// <param name="riderConfiguration">Extension method for IRiderRegistrationConfigurator</param>
    /// <param name="assemblies">Parameter of List of project assemblies</param>
    public static void AddProducers(this IRiderRegistrationConfigurator riderConfiguration, Assembly[] assemblies)
    {
        var allKafkaProducers = assemblies.GetAllKafkaProducers();
        foreach (var kafkaProducer in allKafkaProducers)
        {
            GetConfiguration(assemblies, kafkaProducer, out var producerConfig,  out var @delegate,out var topicName);
            ProducerConfigure(riderConfiguration, kafkaProducer,topicName, producerConfig, @delegate);
        }
    }
    /// <summary>
    /// Create Configuration if IKafkaProducerConfiguration for Producer is exists
    /// </summary>
    /// <param name="assemblies">Parameter of List of project assemblies</param>
    /// <param name="kafkaProducer">Parameter of kafka producer assembly type</param>
    /// <param name="producerConfig">out producer config</param>
    /// <param name="delegate">out action method of IRiderRegistrationContext, IKafkaProducerConfigurator</param>
    private static void GetConfiguration(Assembly[] assemblies, Type kafkaProducer, out ProducerConfig? producerConfig,
        out Delegate? @delegate, out object topicName)
    {
        var allKafkaConfiguration = GetAllKafkaProducerConfigs(assemblies, kafkaProducer);

        var makeGenericIKafkaProducerConfiguratorAction = typeof(Action<,>)
            .MakeGenericType(typeof(IRiderRegistrationContext),
                typeof(IKafkaProducerConfigurator<,>).MakeGenericType(typeof(Null), kafkaProducer));

        if (allKafkaConfiguration == null)
        {
            producerConfig = null;
            @delegate = null;
            topicName = null;
            return;
        }

        var instance = Activator.CreateInstance(allKafkaConfiguration);


        topicName = instance.GetType().GetProperty("TopicName")?.GetValue(instance);
        var actionMethod = allKafkaConfiguration.GetMethod(KafkaProducerConfigurationMethod);

        producerConfig = instance?.GetType().GetProperty(nameof(ProducerConfig))?.GetValue(instance) as ProducerConfig;

        @delegate = Delegate.CreateDelegate(makeGenericIKafkaProducerConfiguratorAction, instance, actionMethod);
    }

    /// <summary>
    /// Reflecting on the rider extension method
    /// </summary>
    /// <param name="riderConfiguration">Extension method for IRiderRegistrationConfigurator</param>
    /// <param name="kafkaProducer">Parameter of kafka producer assembly type</param>
    /// <param name="producerConfig">Parameter of producer producer config</param>
    /// <param name="delegate">Parameter of action method of IRiderRegistrationContext, IKafkaProducerConfigurator </param>
    private static void ProducerConfigure(this IRiderRegistrationConfigurator riderConfiguration, Type kafkaProducer, object topicName,
        ProducerConfig? producerConfig, Delegate? @delegate = null)
    {
        typeof(RiderProducerExtensions).GetMethod(nameof(RiderExtension))
            ?.MakeGenericMethod(kafkaProducer)
            .Invoke(kafkaProducer, new object[]
            {
                riderConfiguration,
                string.IsNullOrWhiteSpace(topicName.ToString())? kafkaProducer.Name : topicName,
                producerConfig,
                @delegate
            });
    }

    /// <summary>
    /// Set AddProducer after look up producers
    /// </summary>
    /// <param name="configurator">Create IRiderRegistrationConfigurator extension method to easily handled add producer in reflection</param>
    /// <param name="topicName">Parameter of topic name from class name</param>
    /// <param name="producerConfig">Parameter of producer config if it exists</param>
    /// <param name="configure">Parameter of configuration for producer and set it to the MassTransit add producer</param>
    /// <typeparam name="TProducer">Parameter of producer implementation</typeparam>
    public static void RiderExtension<TProducer>(this IRiderRegistrationConfigurator configurator, string topicName,
        ProducerConfig? producerConfig,
        Action<IRiderRegistrationContext, IKafkaProducerConfigurator<Null, TProducer>>? configure = null)
        where TProducer : class, IKafkaProducer
    {
        if (producerConfig != null)
            configurator.AddProducer(topicName, producerConfig, configure);

        configurator.AddProducer(topicName, configure);
    }

    /// <summary>
    /// Get all producers from IKafkaProducer to register in masstransit add producer provider
    /// </summary>
    /// <param name="assemblies"> Parameter of List of project assemblies</param>
    /// <returns></returns>
    private static IEnumerable<Type> GetAllKafkaProducers(this IEnumerable<Assembly> assemblies)
    {
        // get all classes that implements "IProducer" interface from loadable assemblies
        return assemblies
            .SelectMany(assemblies => assemblies.GetTypes()).Where(type =>
                type.GetInterfaces().Contains(typeof(IKafkaProducer))).AsEnumerable();
    }

    /// <summary>
    /// Get all kafka producer from IKafkaProducerConfiguration implementation
    /// </summary>
    /// <param name="assemblies">Parameter of List of project assemblies</param>
    /// <param name="assembly">Parameter of producer assembly</param>
    /// <returns></returns>
    private static Type? GetAllKafkaProducerConfigs(this IEnumerable<Assembly> assemblies, Type assembly)
    {
        return assemblies
            .SelectMany(listOfAssemblies => listOfAssemblies.GetTypes()).FirstOrDefault(type => type.BaseType is {IsGenericType: true} &&
                type.BaseType.GetGenericTypeDefinition() == (typeof(KafkaProducerConfiguration<>)) &&
                                              type.BaseType.GetGenericArguments()[0].FullName.Contains(assembly.FullName));
    }

    /// <summary>
    /// Separates the input words with underscore
    /// </summary>
    /// <param name="input">The string to be underscored</param>
    /// <returns></returns>
    public static string Underscore(this string input)
    {
        return Regex.Replace(
            Regex.Replace(
                Regex.Replace(input, @"([\p{Lu}]+)([\p{Lu}][\p{Ll}])", "$1_$2"), @"([\p{Ll}\d])([\p{Lu}])", "$1_$2"),
            @"[-\s]", "_").ToLower();
    }
}
}
