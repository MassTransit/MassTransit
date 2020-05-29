namespace MassTransit.Riders
{
    using GreenPipes;


    public interface IRiderConnector
    {
        ConnectHandle ConnectRider(IRider rider);
    }
}
