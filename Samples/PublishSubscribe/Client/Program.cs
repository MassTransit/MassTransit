namespace Client
{
    using log4net.Config;
    using MassTransit.Host2;

    internal class Program
    {
        private static void Main(string[] args)
        {
            XmlConfigurator.Configure();

            HostedEnvironment env = new ClientEnvironment("castle.xml");
            Runner.Run(env, args);
        }
    }
}