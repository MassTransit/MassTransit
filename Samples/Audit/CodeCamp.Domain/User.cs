namespace CodeCamp.Domain
{
    using Core;
    using Messages;

    public class User : IIdentifier
    {
        private readonly string _email;
        private readonly string _name;
        private readonly string _password;
        private readonly string _username;
        private bool? _hasEmailBeenConfirmed;

        public User(string name, string username, string password, string email)
        {
            _name = name;
            _username = username;
            _password = password;
            _email = email;
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

        #region IIdentifier Members

        public object Key
        {
            get { return _username; }
        }

        #endregion

        public bool CheckPassword(string password)
        {
            if (_password == password)
            {
                DomainContext.Publish(new UserPasswordSuccess(_username));
                return true;
            }

            DomainContext.Publish(new UserPasswordFailure(_username));

            return false;
        }

        public void EmailHasBeenConfirmed()
        {
            _hasEmailBeenConfirmed = true;
        }

        public void SetPending()
        {
            _hasEmailBeenConfirmed = false;
        }
    }
}