namespace MassTransit.Definition
{
    using System;
    using System.Text;
    using Courier;
    using Metadata;
    using Saga;
    using Util;


    /// <summary>
    /// The default endpoint name formatter, which simply trims the words Consumer, Activity, and Saga
    /// from the type name. If you need something more readable, consider the <see cref="SnakeCaseEndpointNameFormatter"/>
    /// or the <see cref="KebabCaseEndpointNameFormatter"/>.
    /// </summary>
    public class DefaultEndpointNameFormatter :
        IEndpointNameFormatter
    {
        protected DefaultEndpointNameFormatter()
        {
        }

        public static IEndpointNameFormatter Instance { get; } = new DefaultEndpointNameFormatter();

        public string TemporaryEndpoint(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
                tag = "endpoint";

            var host = HostMetadataCache.Host;

            var sb = new StringBuilder(host.MachineName.Length + host.ProcessName.Length + tag.Length + 35);

            foreach (var c in host.MachineName)
            {
                if (char.IsLetterOrDigit(c) || c == '_')
                    sb.Append(c);
            }

            sb.Append('_');
            foreach (var c in host.ProcessName)
            {
                if (char.IsLetterOrDigit(c) || c == '_')
                    sb.Append(c);
            }

            sb.Append('_');
            sb.Append(tag);
            sb.Append('_');
            sb.Append(NewId.Next().ToString(FormatUtil.Formatter));

            return sb.ToString();
        }

        public string Consumer<T>()
            where T : class, IConsumer
        {
            return GetConsumerName(typeof(T).Name);
        }

        public string Saga<T>()
            where T : class, ISaga
        {
            return GetSagaName(typeof(T).Name);
        }

        public string ExecuteActivity<T, TArguments>()
            where T : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            var activityName = GetActivityName(typeof(T).Name);

            return $"{activityName}_execute";
        }

        public string CompensateActivity<T, TLog>()
            where T : class, ICompensateActivity<TLog>
            where TLog : class
        {
            var activityName = GetActivityName(typeof(T).Name);

            return $"{activityName}_compensate";
        }

        string GetConsumerName(string typeName)
        {
            const string consumer = "Consumer";

            var consumerName = typeName;
            if (consumerName.EndsWith(consumer, StringComparison.InvariantCultureIgnoreCase))
                consumerName = consumerName.Substring(0, consumerName.Length - consumer.Length);

            return SanitizeName(consumerName);
        }

        string GetSagaName(string typeName)
        {
            const string saga = "Saga";

            var sagaName = typeName;
            if (sagaName.EndsWith(saga, StringComparison.InvariantCultureIgnoreCase))
                sagaName = sagaName.Substring(0, sagaName.Length - saga.Length);

            return SanitizeName(sagaName);
        }

        string GetActivityName(string typeName)
        {
            const string activity = "Activity";

            var activityName = typeName;
            if (activityName.EndsWith(activity, StringComparison.InvariantCultureIgnoreCase))
                activityName = activityName.Substring(0, activityName.Length - activity.Length);

            return SanitizeName(activityName);
        }

        protected virtual string SanitizeName(string name)
        {
            return name;
        }
    }
}
