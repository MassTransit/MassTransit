namespace Server
{
    using MassTransit.Host2;

    public class ServerEnvironment :
        HostedEnvironment
    {
        public ServerEnvironment()
        {
        }

        public ServerEnvironment(string xmlFile) : base(xmlFile)
        {
        }

        public override string ServiceName
        {
            get { return "Server"; }
        }

        public override string DispalyName
        {
            get { return "Mass Transit Test Server"; }
        }

        public override string Description
        {
            get { return "Serves it up"; }
        }
    }
}