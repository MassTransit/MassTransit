namespace MassTransit
{
    using GreenPipes;


    public interface IReceivePipeConfigurator :
        IPipeConfigurator<ReceiveContext>
    {
    }
}
