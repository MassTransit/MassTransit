namespace MassTransit.Conductor.Directory
{
    public interface IConfigureServiceDirectory
    {
        void Configure(IServiceDirectoryConfigurator configurator);
    }
}
