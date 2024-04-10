namespace MassTransit
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
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

        /// <summary>
        /// Default endpoint name formatter.
        /// </summary>
        /// <param name="includeNamespace">If true, the namespace is included in the name</param>
        public DefaultEndpointNameFormatter(bool includeNamespace)
        {
            IncludeNamespace = includeNamespace;
        }

        /// <summary>
        /// Default endpoint name formatter with prefix.
        /// </summary>
        /// <param name="prefix">Prefix to start the name, should match the casing of the formatter (such as Dev or PreProd)</param>
        /// <param name="includeNamespace">If true, the namespace is included in the name</param>
        public DefaultEndpointNameFormatter(string prefix, bool includeNamespace)
        {
            Prefix = prefix;
            IncludeNamespace = includeNamespace;
        }

        /// <summary>
        /// Default endpoint name formatter with prefix.
        /// </summary>
        /// <param name="prefix">Prefix to start the name, should match the casing of the formatter (such as Dev or PreProd)</param>
        public DefaultEndpointNameFormatter(string prefix)
        {
            Prefix = prefix;
            IncludeNamespace = false;
        }

        /// <summary>
        /// Default endpoint name formatter with join separator and prefix.
        /// </summary>
        /// <param name="joinSeparator">Define the join separator between the words</param>
        /// <param name="prefix">Prefix to start the name, should match the casing of the formatter (such as Dev or PreProd)</param>
        /// <param name="includeNamespace">If true, the namespace is included in the name</param>
        public DefaultEndpointNameFormatter(string joinSeparator, string prefix, bool includeNamespace)
        {
            Prefix = prefix;
            IncludeNamespace = includeNamespace;
            JoinSeparator = joinSeparator;
        }

        protected DefaultEndpointNameFormatter()
        {
            IncludeNamespace = false;
        }

        /// <summary>
        /// Gets a value indicating whether the namespace is included in the name.
        /// </summary>
        protected bool IncludeNamespace { get; }

        /// <summary>
        /// Gets the Prefix to start the name.
        /// </summary>
        protected string Prefix { get; }

        /// <summary>
        /// Gets the join separator between the words
        /// </summary>
        protected string JoinSeparator { get; }

        public static IEndpointNameFormatter Instance { get; } = new DefaultEndpointNameFormatter();

        public string Separator { get; protected set; } = "";

        public virtual string TemporaryEndpoint(string tag)
        {
            return GetTemporaryQueueName(tag);
        }

        public virtual string Consumer<T>()
            where T : class, IConsumer
        {
            return GetConsumerName(typeof(T));
        }

        public virtual string Message<T>()
            where T : class
        {
            return GetMessageName(typeof(T));
        }

        public virtual string Saga<T>()
            where T : class, ISaga
        {
            return GetSagaName(typeof(T));
        }

        public virtual string ExecuteActivity<T, TArguments>()
            where T : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            var activityName = GetActivityName(typeof(T), typeof(TArguments));

            return $"{activityName}_execute";
        }

        public virtual string CompensateActivity<T, TLog>()
            where T : class, ICompensateActivity<TLog>
            where TLog : class
        {
            var activityName = GetActivityName(typeof(T), typeof(TLog));

            return $"{activityName}_compensate";
        }

        public virtual string SanitizeName(string name)
        {
            return name;
        }

        public static string GetTemporaryQueueName(string tag)
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

        /// <summary>
        /// Gets the endpoint name for a consumer of the given type.
        /// </summary>
        /// <param name="type">The type of the consumer implementing <see cref="IConsumer" /></param>
        /// <returns>The fully formatted name as it will be provided via <see cref="Consumer{T}" /></returns>
        protected virtual string GetConsumerName(Type type)
        {
            if (type.IsGenericType && type.Name.Contains('`'))
                return SanitizeName(FormatName(type.GetGenericArguments().Last()));

            const string consumer = "Consumer";

            var consumerName = FormatName(type);

            if (consumerName.EndsWith(consumer, StringComparison.InvariantCultureIgnoreCase))
            {
                consumerName = consumerName.Substring(0, consumerName.Length - consumer.Length);

                if (string.IsNullOrWhiteSpace(consumerName))
                    throw new ConfigurationException($"A consumer may not be named \"{consumer}\". Add a meaningful prefix when using ConfigureEndpoints.");
            }

            return SanitizeName(consumerName);
        }

        /// <summary>
        /// Gets the endpoint name for a message of the given type.
        /// </summary>
        /// <param name="type">The type of the message</param>
        /// <returns>The fully formatted name as it will be provided via <see cref="Message{T}" /></returns>
        protected virtual string GetMessageName(Type type)
        {
            if (type.IsGenericType && type.Name.Contains('`'))
                return SanitizeName(FormatName(type.GetGenericArguments().Last()));

            var messageName = FormatName(type);

            return SanitizeName(messageName);
        }

        /// <summary>
        /// Gets the endpoint name for a saga of the given type.
        /// </summary>
        /// <param name="type">The type of the saga implementing <see cref="ISaga" /></param>
        /// <returns>The fully formatted name as it will be provided via <see cref="Saga{T}" /></returns>
        protected virtual string GetSagaName(Type type)
        {
            const string saga = "Saga";

            var sagaName = FormatName(type);

            if (sagaName.EndsWith(saga, StringComparison.InvariantCultureIgnoreCase))
            {
                sagaName = sagaName.Substring(0, sagaName.Length - saga.Length);
                if (string.IsNullOrWhiteSpace(sagaName))
                    throw new ConfigurationException($"A saga may not be named \"{saga}\". Add a meaningful prefix when using ConfigureEndpoints.");
            }

            return SanitizeName(sagaName);
        }

        /// <summary>
        /// Gets the name for an activity of the given type.
        /// </summary>
        /// <remarks>
        /// The activity name is used both for execution and compensation endpoint names.
        /// </remarks>
        /// <param name="activityType">The type of the activity implementing <see cref="IActivity" /></param>
        /// <param name="argumentType">
        /// For execution endpoints this is the activity arguments, for compensation this is the log type.
        /// </param>
        /// <returns>The formatted activity name further used in <see cref="ExecuteActivity{T,TArguments}" /> and <see cref="CompensateActivity{T,TLog}" />.</returns>
        protected virtual string GetActivityName(Type activityType, Type argumentType)
        {
            const string activity = "Activity";

            var activityName = FormatName(activityType);

            if (activityName.EndsWith(activity, StringComparison.InvariantCultureIgnoreCase))
            {
                activityName = activityName.Substring(0, activityName.Length - activity.Length);
                if (string.IsNullOrWhiteSpace(activityName))
                    throw new ConfigurationException($"An activity may not be named \"{activity}\". Add a meaningful prefix when using ConfigureEndpoints.");
            }

            return SanitizeName(activityName);
        }

        /// <summary>
        /// Does a basic formatting of the type respecting settings like <see cref="IncludeNamespace" />.
        /// </summary>
        /// <param name="type">The type to format.</param>
        /// <returns>A formatted type name, not yet sanitized via <see cref="SanitizeName" />.</returns>
        protected virtual string FormatName(Type type)
        {
            var name = IncludeNamespace
                ? string.Join(JoinSeparator ?? "", TypeCache.GetShortName(type).Split(_removeChars))
                : type.Name;

            return string.IsNullOrWhiteSpace(Prefix) ? name : Prefix + name;
        }
    }
}
