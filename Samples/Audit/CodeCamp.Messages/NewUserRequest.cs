namespace CodeCamp.Messages
{
    using System;

    [Serializable]
    public class NewUserRequest
    {
        private readonly string _name;
        private readonly string _username;
        private readonly string _password;

        public NewUserRequest(string name, string username, string password)
        {
            _name = name;
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
    }
}