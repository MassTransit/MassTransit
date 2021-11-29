namespace MassTransit.Observables
{
    using System;
    using System.Threading.Tasks;


    /// <summary>
    /// Calls the generic version of the IPublishEndpoint.Send method with the object's type
    /// </summary>
    public interface IConsumeObserverConverter
    {
        Task PreConsume(IConsumeObserver observer, object context);

        Task PostConsume(IConsumeObserver observer, object context);

        Task ConsumeFault(IConsumeObserver observer, object context, Exception exception);
    }
}
