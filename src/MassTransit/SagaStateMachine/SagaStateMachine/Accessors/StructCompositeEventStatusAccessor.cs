namespace MassTransit.SagaStateMachine
{
    using System.Reflection;
    using Internals;


    public class StructCompositeEventStatusAccessor<TSaga> :
        ICompositeEventStatusAccessor<TSaga>
        where TSaga : class
    {
        readonly string _name;
        readonly IReadProperty<TSaga, CompositeEventStatus> _read;
        readonly IWriteProperty<TSaga, CompositeEventStatus> _write;

        public StructCompositeEventStatusAccessor(PropertyInfo propertyInfo)
        {
            _read = ReadPropertyCache<TSaga>.GetProperty<CompositeEventStatus>(propertyInfo);
            _write = WritePropertyCache<TSaga>.GetProperty<CompositeEventStatus>(propertyInfo);
            _name = propertyInfo.Name;
        }

        public CompositeEventStatus Get(TSaga instance)
        {
            return _read.Get(instance);
        }

        public void Set(TSaga instance, CompositeEventStatus status)
        {
            _write.Set(instance, status);
        }

        public void Probe(ProbeContext context)
        {
            context.Add("property", _name);
            context.Add("type", nameof(CompositeEventStatus));
        }
    }
}
