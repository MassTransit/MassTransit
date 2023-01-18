namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Confluent.Kafka;


    public static class KafkaTopicEndPointExtensions
{
    // Group Id property name from class TopicEndPoint
    const string GroupId = "GroupId";

    // Action method name from class TopicEndPoint
    const string Method = "ActionMethod";

    /// <summary>
    /// Add TopicEndPoints dynamically
    /// </summary>
    /// <param name="configurator">Extension method for IKafkaFactoryConfigurator</param>
    /// <param name="riderRegistrationContext">Get IRiderRegistrationContext value</param>
    /// <param name="assemblies">Parameter of List of project assemblies</param>
    public static void AddTopicEndPoints(this IKafkaFactoryConfigurator configurator,
        IRiderRegistrationContext riderRegistrationContext, IEnumerable<Assembly> assemblies)
    {
        var allTypes = assemblies.GetAllKafkaProducerTopicEndPointConfig();
        foreach (var assembly in allTypes)
        {
            GetTopicEndpointConfiguration(riderRegistrationContext, assembly, out var groupName, out var topicName,
                out var @delegate);

            typeof(KafkaTopicEndPointExtensions).GetMethod(nameof(KafkaExtension))
                ?.MakeGenericMethod(assembly.BaseType.GetGenericArguments()[0])
                .Invoke(assembly, new[]
                {
                    configurator,
                    topicName,
                    groupName,
                    @delegate
                });
        }
    }

    /// <summary>
    /// Create Action for IKafkaTopicReceiveEndpointConfigurator
    /// </summary>
    /// <param name="riderRegistrationContext">Parameter for IRiderRegistrationContext</param>
    /// <param name="assembly">Parameter of assembly of Topic End point configuration implementation</param>
    /// <param name="groupName">Out group name of topic</param>
    /// <param name="topicName">Out topic name and it's retrieved from inherited class from IKafkaProducer name</param>
    /// <param name="delegate">Out Action for IKafkaTopicReceiveEndpointConfigurator</param>
    static void GetTopicEndpointConfiguration(IRiderRegistrationContext riderRegistrationContext, Type assembly,
        out object groupName, out string topicName, out Delegate @delegate)
    {
        var actionIKafkaTopicReceiveEndpointConfigurator = typeof(Action<>).MakeGenericType(
            typeof(IKafkaTopicReceiveEndpointConfigurator<,>).MakeGenericType(typeof(Ignore),
                assembly.BaseType.GetGenericArguments()[0]));

        var instance = Activator.CreateInstance(assembly, args: riderRegistrationContext);

        groupName = instance.GetType().GetProperty(GroupId)?.GetValue(instance);

        topicName = assembly.BaseType.GetGenericArguments()[0].Name.Underscore();

        var actionMethod = assembly.GetMethod(Method, BindingFlags.Instance | BindingFlags.NonPublic);


        @delegate = Delegate.CreateDelegate(actionIKafkaTopicReceiveEndpointConfigurator, instance, actionMethod);
    }

    /// <summary>
    /// Get All Kafka Producer Topic End Point Configuration
    /// </summary>
    /// <param name="assemblies">Parameter of List of project assemblies</param>
    /// <returns></returns>
    private static IEnumerable<Type> GetAllKafkaProducerTopicEndPointConfig(this IEnumerable<Assembly> assemblies)
    {
        // get all classes that implements "TopicEndPoint" interface from loadable assemblies
        var data = assemblies
            .SelectMany(assemblies => assemblies.GetTypes())
            .Where(t => t.BaseType is {IsGenericType: true} &&
                        t.BaseType.GetGenericTypeDefinition() == typeof(TopicEndPoint<>))
            .AsEnumerable();
        return data;
    }

    public static void KafkaExtension<TProducer>(this IKafkaFactoryConfigurator configurator, string topicName,
        string groupId,
        Action<IKafkaTopicReceiveEndpointConfigurator<Ignore, TProducer>> configure)
        where TProducer : class, IKafkaProducer
    {
        configurator.TopicEndpoint<TProducer>(topicName, groupId, configure);
    }
}
}

