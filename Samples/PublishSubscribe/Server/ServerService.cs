namespace Server
{
    using MassTransit;
    using Microsoft.Practices.ServiceLocation;

    public class ServerService
    {
        public void Start()
        {
            var hs = ServiceLocator.Current.GetAllInstances<IHostedService>();
            foreach (var hostedService in hs)
            {
                hostedService.Start();
            }
        }

        public void Stop()
        {
            var hs = ServiceLocator.Current.GetAllInstances<IHostedService>();
            foreach (var hostedService in hs)
            {
                hostedService.Stop();
            }
        }
        
    }
}