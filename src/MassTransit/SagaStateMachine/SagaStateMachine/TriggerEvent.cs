namespace MassTransit.SagaStateMachine
{
    using System;


    public class TriggerEvent :
        Event
    {
        readonly string _name;
        public bool IsComposite { get; set; }

        public TriggerEvent(string name, bool isComposite = false)
        {
            _name = name;
            IsComposite = isComposite;
        }

        public string Name => _name;

        public virtual void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this, x =>
            {
            });
        }

        public virtual void Probe(ProbeContext context)
        {
            context.Add("name", _name);
        }

        public int CompareTo(Event other)
        {
            return string.Compare(_name, other.Name, StringComparison.Ordinal);
        }

        public bool Equals(TriggerEvent other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return Equals(other._name, _name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != typeof(TriggerEvent))
                return false;
            return Equals((TriggerEvent)obj);
        }

        public override int GetHashCode()
        {
            return _name?.GetHashCode() ?? 0;
        }

        public override string ToString()
        {
            return $"{_name} (Event)";
        }
    }
}
