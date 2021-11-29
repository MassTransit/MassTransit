namespace MassTransit.MongoDbIntegration.Saga
{
    using System.Text.RegularExpressions;


    /// <summary>
    /// Formats the saga collection names using dot naming with Pluralize
    /// SimpleSaga -> simple.sagas
    /// </summary>
    public class DotCaseCollectionNameFormatter :
        ICollectionNameFormatter
    {
        static readonly Regex Pattern;

        public static readonly ICollectionNameFormatter Instance = new DotCaseCollectionNameFormatter();

        static DotCaseCollectionNameFormatter()
        {
            Pattern = new Regex("(?<=[a-z0-9])[A-Z]", RegexOptions.Compiled);
        }

        public string Saga<TSaga>()
            where TSaga : ISaga
        {
            return Pluralize(Pattern.Replace(typeof(TSaga).Name, m => $".{m.Value}")).ToLowerInvariant();
        }

        static string Pluralize(string str)
        {
            if (string.IsNullOrEmpty(str))
                return null;

            if (str.EndsWith("s"))
                return str;

            if (str.EndsWith("y"))
                return str.Substring(0, str.Length - 1) + "ies";

            return str + "s";
        }
    }
}
