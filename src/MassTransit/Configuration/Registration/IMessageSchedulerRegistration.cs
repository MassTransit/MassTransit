namespace MassTransit.Registration
{
    public interface IMessageSchedulerRegistration
    {
        void Register(IContainerRegistrar registrar);
    }
}
