namespace CodeCamp.Service.Domain
{
    public class User
    {
        private bool _pendingEmailConfirmation;

        public User(string name, string username, string password)
        {
            Name = name;
            Username = username;
            Password = password;
        }

        public string Name { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool PendingEmailConfirmation { get
        {
            return _pendingEmailConfirmation;
        } }
        public void SetPending()
        {
            _pendingEmailConfirmation = true;
        }

        public void EmailHasBeenConfirmed()
        {
            _pendingEmailConfirmation = false;
        }
    }
}