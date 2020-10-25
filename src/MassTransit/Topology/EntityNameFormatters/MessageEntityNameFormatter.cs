namespace MassTransit.Topology.EntityNameFormatters
{
    using System.Reflection;


    public class MessageEntityNameFormatter<TMessage> :
        IMessageEntityNameFormatter<TMessage>
        where TMessage : class
    {
        readonly IEntityNameFormatter _entityNameFormatter;
        string _entityName;

        public MessageEntityNameFormatter(IEntityNameFormatter entityNameFormatter)
        {
            _entityNameFormatter = entityNameFormatter;

            InitializeEntityNameFromAttributeIfSpecified();
        }

        /// <summary>
        /// Not sure it ever makes sense to pass the actual message, but many, someday.
        /// </summary>
        /// <returns></returns>
        public string FormatEntityName()
        {
            return _entityName ??= _entityNameFormatter.FormatEntityName<TMessage>();
        }

        void InitializeEntityNameFromAttributeIfSpecified()
        {
            var entityNameAttribute = typeof(TMessage).GetCustomAttribute<EntityNameAttribute>();
            if (entityNameAttribute != null)
                _entityName = entityNameAttribute.EntityName;
        }
    }
}
