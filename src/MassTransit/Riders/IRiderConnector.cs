namespace MassTransit.Riders
{
    using GreenPipes;


    public interface IRiderConnector
    {
        ConnectHandle Connect(IRider rider);
    }
}
