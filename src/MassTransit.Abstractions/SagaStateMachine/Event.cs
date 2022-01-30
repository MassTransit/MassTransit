namespace MassTransit
{
    using System;


    public interface Event :
        IVisitable,
        IComparable<Event>
    {
        string Name { get; }
        bool IsComposite { get; set; }
    }


    public interface Event<out TMessage> :
        Event
        where TMessage : class
    {
    }
}
