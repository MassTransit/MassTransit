namespace MassTransit.MessageData.Values
{
    using System;
    using System.Threading.Tasks;


    /// <summary>
    /// MessageData that has been stored by the repository, has a valid address, and is ready to
    /// be serialized.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class StoredMessageData<T> :
        MessageData<T>
    {
        public StoredMessageData(Uri address, T value)
        {
            Address = address;
            Value = Task.FromResult(value);
        }

        public Uri Address { get; }

        public bool HasValue => true;

        public Task<T> Value { get; }
    }
}
