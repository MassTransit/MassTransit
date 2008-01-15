namespace MassTransit.ServiceBus.SubscriptionsManager
{
    public class StoredSubscription
    {
        private int _id;
        private string _address;
        private string _message;
        private bool _isActive;

        /// <summary>
        /// For NHibernate
        /// </summary>
        private StoredSubscription()
        {
        }

        public StoredSubscription(string address, string message)
        {
            _id = 0;
            _address = address;
            _message = message;
            _isActive = true;
        }

        public int Id
        {
            get { return _id; }
        }

        public string Address
        {
            get { return _address; }
        }

        public string Message
        {
            get { return _message; }
        }

        public bool IsActive
        {
            get { return _isActive; }
            set { _isActive = value; }
        }
    }
}