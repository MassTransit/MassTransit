namespace PostalService
{
    using System.IO;
    using Host;
    using log4net.Config;
    using MassTransit.WindsorIntegration;
    using Topshelf;
    using Topshelf.Configuration;

    internal class Program
    {
        private static void Main(string[] args)
        {
            XmlConfigurator.ConfigureAndWatch(new FileInfo("log4net.xml"));

            var cfg = RunnerConfigurator.New(c =>
                                                 {
                                                     c.SetServiceName("PostalService");
                                                     c.SetDisplayName("Sample Email Service");
                                                     c.SetDescription("we goin' postal");

                                                     c.RunAsLocalSystem();
                                                     c.DependencyOnMsmq();

                                                     c.BeforeStart(a =>
                                                     {
                                                         var container = new DefaultMassTransitContainer("postal-castle.xml");
                                                         container.AddComponent<SendEmailConsumer>("sec");
                                                     });

                                                     c.ConfigureService<PostalServiceLifeCycle>();
                                                 });
            Runner.Host(cfg, args);
        }
    }
}