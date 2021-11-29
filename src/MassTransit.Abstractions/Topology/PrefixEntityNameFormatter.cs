namespace MassTransit
{
    public class PrefixEntityNameFormatter :
        IEntityNameFormatter
    {
        readonly IEntityNameFormatter _entityNameFormatter;
        readonly string _prefix;

        public PrefixEntityNameFormatter(IEntityNameFormatter entityNameFormatter, string prefix)
        {
            _entityNameFormatter = entityNameFormatter;
            _prefix = prefix;
        }

        public string FormatEntityName<T>()
        {
            var name = _entityNameFormatter.FormatEntityName<T>();

            return $"{_prefix}{name}";
        }
    }
}
