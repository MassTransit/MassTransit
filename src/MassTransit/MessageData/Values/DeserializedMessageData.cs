namespace MassTransit.MessageData.Values
{
    using System;
    using System.Threading.Tasks;


    /// <summary>
    /// When a message data property is deserialized, this is used as a placeholder for the actual message
    /// data accessor which replaces this property value once the message is transformed on the pipeline.
    /// </summary>
    /// <typeparam name="T">
    /// The type used to access the message data, valid types include stream, string, and byte[].
    /// </typeparam>
    public class DeserializedMessageData<T> :
        MessageData<T>
    {
        public DeserializedMessageData(Uri address)
        {
            Address = address;
            HasValue = true;
        }

        public Uri Address { get; }
        public bool HasValue { get; }

        public Task<T> Value
        {
            get
            {
                if (HasValue == false)
                    throw new MessageDataException("The message data has no value");

                throw new MessageDataException("The message data was not loaded: " + Address);
            }
        }
    }
}
