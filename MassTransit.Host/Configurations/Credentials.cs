namespace MassTransit.Host.Configurations
{
    using System;

    public class Credentials : IEquatable<Credentials>
    {
        private readonly string _username;
        private readonly string _password;

        public Credentials(string username, string password)
        {
            _username = username;
            _password = password;
        }

        public string Username
        {
            get { return _username; }
        }

        public string Password
        {
            get { return _password; }
        }

        public static Credentials NetworkService
        {
            get
            {
                return new Credentials("","");
            }
        }

        #region System.String Overrides

        public bool Equals(Credentials obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj._username, _username) && Equals(obj._password, _password);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Credentials)) return false;
            return Equals((Credentials) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_username != null ? _username.GetHashCode() : 0)*397) ^ (_password != null ? _password.GetHashCode() : 0);
            }
        }

        #endregion
    }
}