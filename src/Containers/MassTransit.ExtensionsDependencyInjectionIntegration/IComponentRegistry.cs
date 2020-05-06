namespace MassTransit.ExtensionsDependencyInjectionIntegration
{
    public interface IComponentRegistry
    {
        string Name { get; }
        IBusControl BusControl { get; }
    }


    public class ComponentRegistry : IComponentRegistry
    {
        public const string DefaultName = "default";

        public ComponentRegistry(string name, IBusControl busControl)
        {
            Name = name;
            BusControl = busControl;
        }

        public string Name { get; }
        public IBusControl BusControl { get; }
    }
}
