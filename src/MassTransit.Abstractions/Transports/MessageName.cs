namespace MassTransit.Transports
{
    using System;
    using System.Runtime.Serialization;


    /// <summary>
    /// Class encapsulating naming strategies for exchanges corresponding
    /// to message types.
    /// </summary>
    [Serializable]
    public class MessageName :
        ISerializable
    {
        public MessageName(string name)
        {
            Name = name;
        }

        protected MessageName(SerializationInfo info, StreamingContext context)
        {
            Name = info.GetString("Name");
        }

        public string Name { get; }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Name", Name);
        }

        public override string ToString()
        {
            return Name ?? "";
        }
    }
}
