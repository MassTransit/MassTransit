namespace MassTransit.ServiceBus.Saga
{
    using System;

    /// <summary>
    /// Indicates that the object starts a workflow.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class StartsSagaAttribute : Attribute
    {
    }
}