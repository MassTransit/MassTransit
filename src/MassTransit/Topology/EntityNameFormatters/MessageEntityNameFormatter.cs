namespace MassTransit.Topology.EntityNameFormatters
{
    public class MessageEntityNameFormatter<TMessage> :
        IMessageEntityNameFormatter<TMessage>
        where TMessage : class
    {
        readonly IEntityNameFormatter _entityNameFormatter;

        public MessageEntityNameFormatter(IEntityNameFormatter entityNameFormatter)
        {
            _entityNameFormatter = entityNameFormatter;
        }

        /// <summary>
        /// Not sure it ever makes sense to pass the actual message, but many, someday.
        /// </summary>
        /// <returns></returns>
        public string FormatEntityName()
        {
            return _entityNameFormatter.FormatEntityName<TMessage>();
        }
    }
}
