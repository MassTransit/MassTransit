namespace MassTransit.Registration
{
    public interface CachedRegistration
    {
        void Register(IContainerRegistrar registrar);
    }
}
