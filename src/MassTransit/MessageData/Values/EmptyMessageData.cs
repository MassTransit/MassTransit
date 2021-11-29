namespace MassTransit.MessageData.Values
{
    using System;
    using System.Threading.Tasks;


    public class EmptyMessageData<T> :
        MessageData<T>
    {
        public static readonly MessageData<T> Instance = new EmptyMessageData<T>();

        EmptyMessageData()
        {
        }

        public Uri Address => throw new MessageDataException("The message data is empty");

        public bool HasValue => false;

        public Task<T> Value => NoValue();

        static Task<T> NoValue()
        {
            throw new MessageDataException("The message data is empty");
        }
    }
}
