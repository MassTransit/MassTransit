namespace PostalService
{
    using System.IO;
    using Host;
    using MassTransit.Host;
    using MassTransit.Host.Configurations;

    class Program
    {
        static void Main(string[] args)
        {
            //log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo("log4net.xml"));
            log4net.Config.BasicConfigurator.Configure();
            IInstallationConfiguration cfg = new PostalServiceConfiguration("postal-castle.xml");

            Runner.Run(cfg, args);
        }
    }
}
