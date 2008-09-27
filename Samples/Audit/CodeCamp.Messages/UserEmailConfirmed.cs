namespace CodeCamp.Messages
{
    using System;

    [Serializable]
    public class UserEmailConfirmed
    {
        private readonly Guid _registarionId;

        public UserEmailConfirmed(Guid registarionId)
        {
            _registarionId = registarionId;
        }

        public Guid RegistarionId
        {
            get { return _registarionId; }
        }
    }
}