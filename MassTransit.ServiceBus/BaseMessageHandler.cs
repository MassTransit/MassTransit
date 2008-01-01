namespace MassTransit.ServiceBus
{
    public abstract class BaseMessageHandler<T> 
    {
        private IServiceBus _bus;

        public BaseMessageHandler(IServiceBus bus)
        {
            _bus = bus;
        }

        /// <summary>
        /// The service bus form which where messages are being retrieved
        /// </summary>
        public IServiceBus Bus
        {
            get { return _bus; }
            set { _bus = value; }
        }

        #region IMessageEndpoint<T> Members

        /// <summary>
        /// Called by the service bus implementation when a message needs to be handled
        /// </summary>
        /// <param name="message"></param>
        public abstract void Handle(T message);

        #endregion
    }
}