namespace Client
{
    using MassTransit.Host;

    public class ClientEnvironment : HostedEnvironment
    {
        public ClientEnvironment(string xmlFile) : base(xmlFile)
        {
        }

        public override string ServiceName
        {
            get { return "SampleClientService"; }
        }

        public override string DispalyName
        {
            get { return "MassTransit Sample Client Service"; }
        }

        public override string Description
        {
            get { return "Acts as a client on the service bus"; }
        }

        public override HostedLifeCycle LifeCycle
        {
            get { return new ClientLifeCycle(XmlFile); }
        }
    }
}