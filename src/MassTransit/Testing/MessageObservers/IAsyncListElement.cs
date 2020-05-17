namespace MassTransit.Testing.MessageObservers
{
    using System;


    public interface IAsyncListElement
    {
        Guid? ElementId { get; }
    }
}
