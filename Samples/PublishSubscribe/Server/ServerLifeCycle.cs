namespace Server
{
    using MassTransit.Host;

    public class ServerLifeCycle :
        HostedLifeCycle
    {
        public ServerLifeCycle(string xmlFile) : base(xmlFile)
        {
        }

        public override void Start()
        {
            //do nothing
        }

        public override void Stop()
        {
            //do nothing
        }
    }
}