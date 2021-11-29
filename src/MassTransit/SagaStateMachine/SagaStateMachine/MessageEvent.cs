namespace MassTransit.SagaStateMachine
{
    using System;


    public class MessageEvent<TMessage> :
        TriggerEvent,
        Event<TMessage>,
        IEquatable<MessageEvent<TMessage>>
        where TMessage : class
    {
        public static readonly Event<TMessage> Instance = new MessageEvent<TMessage>(TypeCache<TMessage>.ShortName);

        public MessageEvent(string name)
            : base(name)
        {
        }

        public override void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this, x =>
            {
            });
        }

        public override void Probe(ProbeContext context)
        {
            base.Probe(context);

            context.Add("dataType", TypeCache<TMessage>.ShortName);
        }

        public bool Equals(MessageEvent<TMessage> other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return Equals(other.Name, Name);
        }

        public override string ToString()
        {
            return $"{Name}<{typeof(TMessage).Name}> (Event)";
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            return Equals(obj as MessageEvent<TMessage>);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() * 27 + typeof(TMessage).GetHashCode();
        }
    }
}
