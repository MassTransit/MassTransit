namespace MassTransit.Transports.Components
{
    public interface IKillSwitchState :
        IConsumeObserver,
        IProbeSite
    {
    }
}
