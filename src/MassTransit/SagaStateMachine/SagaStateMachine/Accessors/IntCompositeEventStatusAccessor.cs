namespace MassTransit.SagaStateMachine
{
    using System;
    using System.Reflection;
    using Internals;


    public class IntCompositeEventStatusAccessor<TSaga> :
        ICompositeEventStatusAccessor<TSaga>
        where TSaga : class
    {
        readonly string _name;
        readonly IReadProperty<TSaga, int> _read;
        readonly IWriteProperty<TSaga, int> _write;

        public IntCompositeEventStatusAccessor(PropertyInfo propertyInfo)
        {
            _read = ReadPropertyCache<TSaga>.GetProperty<int>(propertyInfo);
            _write = WritePropertyCache<TSaga>.GetProperty<int>(propertyInfo);
            _name = propertyInfo.Name;
        }

        public CompositeEventStatus Get(TSaga instance)
        {
            return new CompositeEventStatus(_read.Get(instance));
        }

        public void Set(TSaga instance, CompositeEventStatus status)
        {
            _write.Set(instance, status.Bits);
        }

        public void Probe(ProbeContext context)
        {
            context.Add("property", _name);
            context.Add("type", nameof(Int32));
        }
    }
}
