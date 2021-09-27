namespace MassTransit.MessageData.Values
{
    using System;
    using System.Threading.Tasks;


    /// <summary>
    /// Message data that needs to be stored in the repository when the message is sent.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PutMessageData<T> :
        MessageData<T>
    {
        public PutMessageData(T value, bool hasValue = true)
        {
            HasValue = hasValue;
            Value = Task.FromResult(value);
        }

        public Uri Address => null;
        public bool HasValue { get; }
        public Task<T> Value { get; }
    }
}
