namespace PostalService
{
    using System.IO;
    using Host;
    using log4net.Config;
    using MassTransit.Host;
    using MassTransit.WindsorIntegration;
    using Microsoft.Practices.ServiceLocation;

    internal class Program
    {
        private static void Main(string[] args)
        {
            XmlConfigurator.ConfigureAndWatch(new FileInfo("log4net.xml"));


            var container = new DefaultMassTransitContainer("postal-castle.xml");
            container.AddComponent<SendEmailConsumer>("sec");

            var credentials = Credentials.LocalSystem;
            var settings = WinServiceSettings.Custom(
                "PostalService",
                "Sample Email Service",
                "We goin' postal",
                KnownServiceNames.Msmq);
            var lifecycle = new PostalServiceLifeCycle(ServiceLocator.Current);

            Runner.Run(credentials,settings,lifecycle, args);
        }
    }
}