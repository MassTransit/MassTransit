namespace MassTransit
{
    using System;


    /// <summary>
    /// Typically added to base messages types, such as IMessage, IEvent, etc.
    /// so that scoped filters are not created on the message type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class ExcludeFromImplementedTypesAttribute :
        Attribute
    {
    }
}
