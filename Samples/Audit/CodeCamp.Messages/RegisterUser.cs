namespace CodeCamp.Messages
{
    using System;
    using MassTransit.ServiceBus;

    [Serializable]
    public class RegisterUser :
        CorrelatedBy<Guid>
    {
        private readonly string _name;
        private readonly string _username;
        private readonly string _password;
        private readonly Guid _correlationId;

        public RegisterUser(string name, string username, string password, Guid correlationId)
        {
            _name = name;
            _correlationId = correlationId;
            _username = username;
            _password = password;
        }

        public string Name
        {
            get { return _name; }
        }

        public string Username
        {
            get { return _username; }
        }

        public string Password
        {
            get { return _password; }
        }

        public Guid CorrelationId
        {
            get { return _correlationId; }
        }
    }
}