namespace SecurityMessages
{
    using System;
    using MassTransit.ServiceBus;

    [Serializable]
    public class PasswordUpdateComplete : IMessage
    {
        private int _errorCode;


        public PasswordUpdateComplete(int errorCode)
        {
            _errorCode = errorCode;
        }


        public int ErrorCode
        {
            get { return _errorCode; }
            set { _errorCode = value; }
        }
    }
}