namespace MassTransit
{
    public class PrefixEntityNameFormatter :
        IEntityNameFormatter
    {
        readonly IEntityNameFormatter _entityNameFormatter;
        readonly string _prefix;
        readonly string _separator;

        public PrefixEntityNameFormatter(
            IEntityNameFormatter entityNameFormatter,
            string prefix,
            string separator = "")
        {
            _entityNameFormatter = entityNameFormatter;
            _prefix = prefix;
            _separator = separator;
        }

        public string FormatEntityName<T>()
        {
            var name = _entityNameFormatter.FormatEntityName<T>();

            return $"{_prefix}{_separator}{name}";
        }
    }
}
