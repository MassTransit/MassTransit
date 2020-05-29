namespace MassTransit.KafkaIntegration.Configuration.Definition
{
    using System.Text.RegularExpressions;


    /// <summary>
    /// Formats the topic names using kebab-case (dashed snake case)
    /// SubmitOrder-> submit-order
    /// OrderState -> order-state
    /// </summary>
    public class KebabCaseTopicNameFormatter :
        DefaultTopicNameFormatter
    {
        static readonly Regex _pattern = new Regex("(?<=[a-z0-9])[A-Z]", RegexOptions.Compiled);
        readonly string _separator;

        KebabCaseTopicNameFormatter()
        {
            _separator = "-";
        }

        public new static ITopicNameFormatter Instance { get; } = new KebabCaseTopicNameFormatter();

        protected override string SanitizeName(string name)
        {
            return base.SanitizeName(_pattern.Replace(name, m => _separator + m.Value));
        }
    }
}
