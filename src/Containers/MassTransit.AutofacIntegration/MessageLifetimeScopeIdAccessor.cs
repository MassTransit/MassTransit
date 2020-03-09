namespace MassTransit.AutofacIntegration
{
    using System.Reflection;
    using Internals.Reflection;


    public class MessageLifetimeScopeIdAccessor<TMessage, TId> :
        ILifetimeScopeIdAccessor<TMessage, TId>
        where TMessage : class
    {
        readonly IReadProperty<TMessage, TId> _property;

        public MessageLifetimeScopeIdAccessor(PropertyInfo propertyInfo)
        {
            _property = ReadPropertyCache<TMessage>.GetProperty<TId>(propertyInfo);
        }

        public bool TryGetScopeId(TMessage input, out TId id)
        {
            if (input != null)
            {
                id = _property.Get(input);
                return true;
            }

            id = default;
            return false;
        }
    }
}
