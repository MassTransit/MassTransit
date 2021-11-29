namespace MassTransit.AzureCosmos.Saga
{
    using System.Text.RegularExpressions;


    /// <summary>
    /// Formats the saga collection names using kebab case naming
    /// SimpleSaga -> simple-saga
    /// </summary>
    public class KebabCaseCollectionIdFormatter :
        ICosmosCollectionIdFormatter
    {
        readonly Regex _pattern;
        readonly string _separator;

        KebabCaseCollectionIdFormatter()
        {
            _separator = "-";
            _pattern = new Regex("(?<=[a-z0-9])[A-Z]", RegexOptions.Compiled);
        }

        public static ICosmosCollectionIdFormatter Instance { get; } = new KebabCaseCollectionIdFormatter();

        public string Saga<TSaga>()
            where TSaga : ISaga
        {
            return _pattern.Replace(typeof(TSaga).Name, m => $"{_separator}{m.Value}").ToLowerInvariant();
        }
    }
}
