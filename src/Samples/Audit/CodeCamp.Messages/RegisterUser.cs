namespace CodeCamp.Messages
{
    using System;
    using MassTransit;

    [Serializable]
    public class RegisterUser :
        CorrelatedBy<Guid>
    {
        private readonly Guid _correlationId;
        private readonly string _email;
        private readonly string _name;
        private readonly string _password;
        private readonly string _username;

        public RegisterUser(Guid correlationId, string name, string username, string password, string email)
        {
            _name = name;
            _email = email;
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

        public string Email
        {
            get { return _email; }
        }

        public Guid CorrelationId
        {
            get { return _correlationId; }
        }
    }
}