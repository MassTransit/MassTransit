namespace Server
{
    using log4net.Config;
    using MassTransit.Host2;

    internal class Program
    {
        private static void Main(string[] args)
        {
            XmlConfigurator.Configure();

            HostedEnvironment env = new ServerEnvironment("castle.xml");

            Runner.Run(env, args);
        }
    }
    
}