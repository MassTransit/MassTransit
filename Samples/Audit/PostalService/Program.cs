namespace PostalService
{
    using Host;
    using MassTransit.Host;
    using MassTransit.Host.Configurations;

    class Program
    {
        static void Main(string[] args)
        {
            IInstallationConfiguration cfg = new PostalServiceConfiguration("postal-castle.xml");

            Runner.Run(cfg, args);
        }
    }
}
