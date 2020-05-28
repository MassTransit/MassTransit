namespace MassTransit
{
    using GreenPipes;


    public interface IBusFactory :
        ISpecification
    {
        /// <summary>
        /// Create the bus, returning the bus control interface
        /// </summary>
        /// <returns></returns>
        IBusControl CreateBus();
    }
}
