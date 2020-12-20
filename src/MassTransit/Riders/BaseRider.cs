namespace MassTransit.Riders
{
    public abstract class BaseRider :
        IRider
    {
        readonly string _name;

        protected BaseRider(string name)
        {
            _name = name;
        }

        protected abstract void AddReceiveEndpoint(IHost cancellationToken);

        public void Connect(IHost host)
        {
            AddReceiveEndpoint(host);
        }
    }
}
