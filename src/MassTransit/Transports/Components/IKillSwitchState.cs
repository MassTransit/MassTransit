namespace MassTransit.Transports.Components
{
    using GreenPipes;


    public interface IKillSwitchState :
        IConsumeObserver,
        IProbeSite
    {
    }
}
