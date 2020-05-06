namespace MassTransit.Registration
{
    public interface IBusRegistration
    {
        string Name { get; }
        IBusControl BusControl { get; }
    }


    public class BusRegistration :
        IBusRegistration
    {
        public BusRegistration(string name, IBusControl busControl)
        {
            Name = name;
            BusControl = busControl;
        }

        public string Name { get; }

        public IBusControl BusControl { get; }
    }
}
