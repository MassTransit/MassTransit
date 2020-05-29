namespace MassTransit.Topology.EntityNameFormatters
{
    public class StaticEntityNameFormatter<TMessage> :
        IMessageEntityNameFormatter<TMessage>
        where TMessage : class
    {
        readonly string _entityName;

        public StaticEntityNameFormatter(string entityName)
        {
            _entityName = entityName;
        }

        string IMessageEntityNameFormatter<TMessage>.FormatEntityName()
        {
            return _entityName;
        }
    }
}
