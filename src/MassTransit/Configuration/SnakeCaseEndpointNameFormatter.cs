namespace MassTransit
{
    using System.Text.RegularExpressions;


    /// <summary>
    /// Formats the endpoint name using snake case. For example,
    /// SubmitOrderConsumer -> submit_order
    /// OrderState -> order_state
    /// UpdateCustomerActivity -> update_customer_execute, update_customer_compensate
    /// </summary>
    public class SnakeCaseEndpointNameFormatter :
        DefaultEndpointNameFormatter
    {
        protected const char SnakeCaseSeparator = '_';

        static readonly Regex _pattern = new Regex("(?<=[a-z0-9])[A-Z]", RegexOptions.Compiled);

        readonly char _separator;

        /// <summary>
        /// Snake case endpoint formatter, which uses underscores between words
        /// </summary>
        /// <param name="includeNamespace">If true, the namespace is included in the name</param>
        public SnakeCaseEndpointNameFormatter(bool includeNamespace)
            : base(includeNamespace)
        {
            _separator = SnakeCaseSeparator;

            Separator = _separator.ToString();
        }

        /// <summary>
        /// Snake case endpoint formatter, which uses underscores between words
        /// </summary>
        /// <param name="prefix">Prefix to start the name, should match the casing of the formatter (such as Dev or PreProd)</param>
        /// <param name="includeNamespace">If true, the namespace is included in the name</param>
        public SnakeCaseEndpointNameFormatter(string prefix, bool includeNamespace)
            : base(prefix, includeNamespace)
        {
            _separator = SnakeCaseSeparator;

            Separator = _separator.ToString();
        }

        /// <summary>
        /// Snake case endpoint formatter, which uses underscores between words
        /// </summary>
        /// <param name="separator">Specify a separator other than _ to separate words</param>
        /// <param name="prefix">Prefix to start the name, should match the casing of the formatter (such as Dev or PreProd)</param>
        /// <param name="includeNamespace">If true, the namespace is included in the name</param>
        public SnakeCaseEndpointNameFormatter(char separator, string prefix, bool includeNamespace)
            : base(prefix, includeNamespace)
        {
            _separator = separator;

            Separator = _separator.ToString();
        }

        protected SnakeCaseEndpointNameFormatter()
        {
            _separator = SnakeCaseSeparator;

            Separator = _separator.ToString();
        }

        public new static IEndpointNameFormatter Instance { get; } = new SnakeCaseEndpointNameFormatter();

        public override string SanitizeName(string name)
        {
            return _pattern.Replace(name, m => _separator + m.Value).ToLowerInvariant();
        }
    }
}
