namespace MassTransit.Monitoring.Health
{
    using Registration;


    public class BusHealth :
    #pragma warning disable 618
        IBusHealth
    {
        readonly IBusControl _busControl;

        public BusHealth(IBusInstance busInstance)
        {
            Name = busInstance.Name;
            _busControl = busInstance.BusControl;
        }

        public string Name { get; }

        public HealthResult CheckHealth()
        {
            return _busControl.CheckHealth();
        }
    }
}
