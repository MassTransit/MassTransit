namespace MassTransit
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using Courier;
    using Metadata;
    using NewIdFormatters;


    /// <summary>
    /// The default endpoint name formatter, which simply trims the words Consumer, Activity, and Saga
    /// from the type name. If you need something more readable, consider the <see cref="SnakeCaseEndpointNameFormatter" />
    /// or the <see cref="KebabCaseEndpointNameFormatter" />.
    /// </summary>
    public class DefaultEndpointNameFormatter :
        IEndpointNameFormatter
    {
        const int MaxTemporaryQueueNameLength = 72;
        const int OverheadLength = 29;
        static readonly char[] _removeChars = { '.', '+' };

        static readonly Regex _nonAlpha = new Regex("[^a-zA-Z0-9]", RegexOptions.Compiled | RegexOptions.Singleline);
        readonly bool _includeNamespace;
        readonly string _prefix;

        /// <summary>
        /// Default endpoint formatter, which does not have a separator between words
        /// </summary>
        /// <param name="includeNamespace">If true, the namespace is included in the name</param>
        public DefaultEndpointNameFormatter(bool includeNamespace)
        {
            _includeNamespace = includeNamespace;
        }

        /// <summary>
        /// Default endpoint formatter, which does not have a separator between words
        /// </summary>
        /// <param name="prefix">Prefix to start the name, should match the casing of the formatter (such as Dev or PreProd)</param>
        /// <param name="includeNamespace">If true, the namespace is included in the name</param>
        public DefaultEndpointNameFormatter(string prefix, bool includeNamespace)
        {
            _prefix = prefix;
            _includeNamespace = includeNamespace;
        }

        protected DefaultEndpointNameFormatter()
        {
            _includeNamespace = false;
        }

        public static IEndpointNameFormatter Instance { get; } = new DefaultEndpointNameFormatter();

        public string Separator { get; protected set; } = "";

        public string TemporaryEndpoint(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
                tag = "endpoint";

            var host = HostMetadataCache.Host;

            var machineName = _nonAlpha.Replace(host.MachineName, "");
            var machineNameLength = machineName.Length;

            var processName = _nonAlpha.Replace(host.ProcessName, "");
            var processNameLength = processName.Length;

            var tagLength = tag.Length;

            var nameLength = machineNameLength + processNameLength + tagLength + OverheadLength;

            var overage = nameLength - MaxTemporaryQueueNameLength;

            const int spread = (MaxTemporaryQueueNameLength - OverheadLength) / 3;

            if (overage > 0 && machineNameLength > spread)
            {
                overage -= machineNameLength - spread;
                machineNameLength = spread;
            }

            if (overage > 0 && processNameLength > spread)
            {
                overage -= processNameLength - spread;
                processNameLength = spread;
            }

            if (overage > 0 && tagLength > spread)
            {
                overage -= tagLength - spread;
                tagLength = spread;
            }

            var sb = new StringBuilder(machineNameLength + processNameLength + tagLength + OverheadLength);

            sb.Append(machineName, 0, machineNameLength);
            sb.Append('_');
            sb.Append(processName, 0, processNameLength);

            sb.Append('_');
            sb.Append(tag, 0, tagLength);
            sb.Append('_');
            sb.Append(NewId.Next().ToString(ZBase32Formatter.LowerCase));

            return sb.ToString();
        }

        public string Consumer<T>()
            where T : class, IConsumer
        {
            return GetConsumerName<T>();
        }

        public string Message<T>()
            where T : class
        {
            return GetMessageName(typeof(T));
        }

        public string Saga<T>()
            where T : class, ISaga
        {
            return GetSagaName<T>();
        }

        public string ExecuteActivity<T, TArguments>()
            where T : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            var activityName = GetActivityName<T>();

            return $"{activityName}_execute";
        }

        public string CompensateActivity<T, TLog>()
            where T : class, ICompensateActivity<TLog>
            where TLog : class
        {
            var activityName = GetActivityName<T>();

            return $"{activityName}_compensate";
        }

        public virtual string SanitizeName(string name)
        {
            return name;
        }

        string GetConsumerName<T>()
        {
            if (typeof(T).IsGenericType && typeof(T).Name.Contains('`'))
                return SanitizeName(FormatName(typeof(T).GetGenericArguments().Last()));

            const string consumer = "Consumer";

            var consumerName = FormatName(typeof(T));

            if (consumerName.EndsWith(consumer, StringComparison.InvariantCultureIgnoreCase))
                consumerName = consumerName.Substring(0, consumerName.Length - consumer.Length);

            return SanitizeName(consumerName);
        }

        string GetMessageName(Type type)
        {
            if (type.IsGenericType && type.Name.Contains('`'))
                return SanitizeName(FormatName(type.GetGenericArguments().Last()));

            var messageName = type.Name;

            return SanitizeName(messageName);
        }

        string GetSagaName<T>()
        {
            const string saga = "Saga";

            var sagaName = FormatName(typeof(T));

            if (sagaName.EndsWith(saga, StringComparison.InvariantCultureIgnoreCase))
                sagaName = sagaName.Substring(0, sagaName.Length - saga.Length);

            return SanitizeName(sagaName);
        }

        string GetActivityName<T>()
        {
            const string activity = "Activity";

            var activityName = FormatName(typeof(T));

            if (activityName.EndsWith(activity, StringComparison.InvariantCultureIgnoreCase))
                activityName = activityName.Substring(0, activityName.Length - activity.Length);

            return SanitizeName(activityName);
        }

        string FormatName(Type type)
        {
            var name = _includeNamespace
                ? string.Join("", TypeCache.GetShortName(type).Split(_removeChars))
                : type.Name;

            return string.IsNullOrWhiteSpace(_prefix) ? name : _prefix + name;
        }
    }
}
