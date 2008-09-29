namespace PostalService.Host
{
    using MassTransit.Host.LifeCycles;

    public class PostalServiceLifeCycle :
        HostedLifeCycle
    {
        public PostalServiceLifeCycle(string xmlFile) : base(xmlFile)
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