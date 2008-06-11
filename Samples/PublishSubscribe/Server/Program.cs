namespace Server
{
    using System.IO;
    using log4net.Config;
    using MassTransit.Host2;

    internal class Program
    {
        private static void Main(string[] args)
        {
            XmlConfigurator.Configure(new FileInfo("log4net.xml"));

            HostedEnvironment env = new ServerEnvironment("castle.xml");

            Runner.Run(env, args);
        }
    }
    
}