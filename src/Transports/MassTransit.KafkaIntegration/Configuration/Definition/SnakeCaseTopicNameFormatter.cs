namespace MassTransit.KafkaIntegration.Definition
{
    using System.Text.RegularExpressions;
    using Topology;


    public class SnakeCaseTopicNameFormatter :
        DefaultTopicNameFormatter
    {
        static readonly Regex _pattern = new Regex("(?<=[a-z0-9])[A-Z]", RegexOptions.Compiled);
        readonly string _separator;

        public SnakeCaseTopicNameFormatter()
        {
            _separator = "_";
        }

        public SnakeCaseTopicNameFormatter(string separator)
        {
            _separator = separator ?? "_";
        }

        public new static IEntityNameFormatter Instance { get; } = new SnakeCaseTopicNameFormatter();

        protected override string SanitizeName(string name)
        {
            return _pattern.Replace(name, m => _separator + m.Value).ToLowerInvariant();
        }
    }
}
