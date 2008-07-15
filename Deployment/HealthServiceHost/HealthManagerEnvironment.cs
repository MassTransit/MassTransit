namespace HealthServiceHost
{
    using MassTransit.Host;

    public class HealthManagerEnvironment :
        HostedEnvironment
    {

        public HealthManagerEnvironment(string xmlFile) 
            : base(xmlFile)
        {
        }


        public override void Main()
        {
            //do nothing
        }

        public override string ServiceName
        {
            get { return "MassTransit Health Manager"; }
        }

        public override string DispalyName
        {
            get { return "MassTransit Health Manager"; }
        }

        public override string Description
        {
            get { return "This service manages the health for Mass Transit"; }
        }
    }
}