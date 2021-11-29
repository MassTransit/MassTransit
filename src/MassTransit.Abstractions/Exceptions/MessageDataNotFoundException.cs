namespace MassTransit
{
    using System;


    [Serializable]
    public class MessageDataNotFoundException :
        MessageDataException
    {
        public MessageDataNotFoundException()
        {
        }

        public MessageDataNotFoundException(Uri address)
            : base($"The message data was not found: {address}")
        {
        }
    }
}
