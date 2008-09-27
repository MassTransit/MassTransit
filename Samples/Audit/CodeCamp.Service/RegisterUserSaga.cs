namespace CodeCamp.Service
{
    using System;

    public class RegisterUserSaga
    {
        private readonly Guid _id;
        private bool _pending;
        private bool _confirmed;

        public RegisterUserSaga(Guid id)
        {
            _id = id;
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
    }
}