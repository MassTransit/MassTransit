namespace Client
{
    using log4net.Config;
    using MassTransit.Host2;
    using MassTransit.ServiceBus;
    using SecurityMessages;

    internal class Program
    {
        private static void Main(string[] args)
        {
            XmlConfigurator.Configure();

            HostedEnvironment env = new ClientEnvironment("castle.xml");
            env.Container.AddComponent<IHostedService, AskPasswordQuestion>();
            env.Container.AddComponent<Consumes<PasswordUpdateComplete>.All, PasswordUpdater>();
            Runner.Run(env, args);
        }
    }
}