namespace MassTransit
{
    using System;
    using System.Reflection;
    using Internals;


    public class MessageEntityNameFormatter<TMessage> :
        IMessageEntityNameFormatter<TMessage>
        where TMessage : class
    {
        readonly IEntityNameFormatter _entityNameFormatter;
        string? _entityName;

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
            else if (typeof(TMessage).ClosesType(typeof(Fault<>), out Type[] messageTypes))
            {
                var faultEntityNameAttribute = messageTypes[0].GetCustomAttribute<FaultEntityNameAttribute>();
                if (faultEntityNameAttribute != null)
                    _entityName = faultEntityNameAttribute.EntityName;
            }
        }
    }
}
