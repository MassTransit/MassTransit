namespace MassTransit.QuartzService.Configuration
{
    public interface IConfigurationProvider
    {
        string GetSetting(string name);
        string GetConnectionString(string name);
    }
}