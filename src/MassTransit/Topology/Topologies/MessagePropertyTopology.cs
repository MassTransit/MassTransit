namespace MassTransit.Topology.Topologies
{
    using System;
    using System.Reflection;


    public class MessagePropertyTopology<TMessage, TProperty> :
        IMessagePropertyTopologyConfigurator<TMessage, TProperty>
        where TMessage : class
    {
        readonly PropertyInfo _propertyInfo;

        public MessagePropertyTopology(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException(nameof(propertyInfo));

            _propertyInfo = propertyInfo;
        }

        public bool IsCorrelationId { get; set; }
    }
}
