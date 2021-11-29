namespace MassTransit.Testing
{
    using System;


    public interface IAsyncListElement
    {
        Guid? ElementId { get; }
    }
}
