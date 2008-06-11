namespace Client
{
    using MassTransit.Host2;
    using MassTransit.ServiceBus;
    using SecurityMessages;

    public class ClientEnvironment : HostedEnvironment
    {
        public ClientEnvironment()
        {
        }

        public ClientEnvironment(string xmlFile) : base(xmlFile)
        {
            Container.AddComponent<IHostedService, AskPasswordQuestion>();
            Container.AddComponent<PasswordUpdater>();

            IServiceBus bus = Container.Resolve<IServiceBus>();
            bus.AddComponent<PasswordUpdateComplete>();
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