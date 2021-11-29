namespace MassTransit
{
    public interface IActiveMqQueueBindingConfigurator :
        IActiveMqQueueConfigurator
    {
        /// <summary>
        /// A routing key for the exchange binding
        /// </summary>
        string Selector { set; }
    }
}
