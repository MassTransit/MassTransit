namespace Client
{
    using MassTransit.Host2;

    public class ClientEnvironment : HostedEnvironment
    {
        public ClientEnvironment()
        {
        }

        public ClientEnvironment(string xmlFile) : base(xmlFile)
        {
        }

        public override string Description
        {
            get { return "Pub Sub Client"; }
        }


        public override string ServiceName
        {
            get { return "PubSubClient"; }
        }

        public override string DispalyName
        {
            get { return "Pub Sub Client"; }
        }
    }
}