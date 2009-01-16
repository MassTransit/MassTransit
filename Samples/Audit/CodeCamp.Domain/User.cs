namespace CodeCamp.Domain
{
    using System;
    using Magnum.Common.Data;
    using Messages;
    using Magnum.Common.ObjectExtensions;

    public class User : 
        IAggregateRoot<Guid>
    {
        private string _email;
        private string _name;
        private string _password;
        private string _username;
        private bool? _hasEmailBeenConfirmed;
        private Guid _id;

        public User(string name, string username, string password, string email)
        {
            _name = name;
            _username = username;
            _password = password;
            _email = email;
        }

        protected User()
        {
        }

        public string Email
        {
            get { return _email; }
        }

        public string Name
        {
            get { return _name; }
        }

        public string Username
        {
            get { return _username; }
        }

        public bool CheckPassword(string password)
        {
            password.MustNotBeNull();
            password.MustNotBeEmpty();

            password = password.Trim();

            if (_password == password)
            {
                DomainContext.Publish(new UserPasswordSuccess(_username));
                return true;
            }

            DomainContext.Publish(new UserPasswordFailure(_username));

            return false;
        }

        public bool? HasEmailBeenConfirmed
        {
            get { return _hasEmailBeenConfirmed; }
        }

        public void ConfirmEmail()
        {
            _hasEmailBeenConfirmed = true;
        }

        public void SetEmailPending()
        {
            _hasEmailBeenConfirmed = false;
        }

        public Guid Id
        {
            get { return _id; }
        }
    }
}