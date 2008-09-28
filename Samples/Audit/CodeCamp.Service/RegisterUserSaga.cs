namespace CodeCamp.Service
{
    using System;
    using Messages;

    public class RegisterUserSaga
    {
        private readonly Guid _id;
        private bool _pending;
        private bool _confirmed;
        private string _name;
        private string _password;
        private string _username;

        public RegisterUserSaga(Guid id, RegisterUser message)
        {
            _id = id;
            _name = message.Name;
            _password = message.Password;
            _username = message.Username;
        }

        public bool Confirmed
        {
            get { return _confirmed; }
        }

        public bool Pending
        {
            get { return _pending; }
        }

        public Guid Id
        {
            get { return _id; }
        }

        public void SetPending()
        {
            _pending = true;
        }

        public void UserHasConfirmedEmail()
        {
            _confirmed = true;
        }

        public string Name
        {
            get { return _name; }
        }

        public string Password
        {
            get { return _password; }
        }

        public string Username
        {
            get { return _username; }
        }
    }
}