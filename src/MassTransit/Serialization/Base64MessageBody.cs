#nullable enable
namespace MassTransit.Serialization
{
    using System;
    using System.IO;


    /// <summary>
    /// Converts a binary-only message body to Base64 so that it can be used with non-binary message transports.
    /// Only the <see cref="GetString"/> method performs the conversion, <see cref="GetBytes"/> and <see cref="GetStream"/>
    /// are pass-through methods.
    /// </summary>
    public class Base64MessageBody :
        MessageBody
    {
        readonly MessageBody _messageBody;
        string? _string;

        public Base64MessageBody(MessageBody messageBody)
        {
            _messageBody = messageBody;
        }

        public long? Length => _string?.Length ?? _messageBody.Length;

        public Stream GetStream()
        {
            return new MemoryStream(GetBytes(), false);
        }

        public byte[] GetBytes()
        {
            return _messageBody.GetBytes();
        }

        public string GetString()
        {
            if (_string != null)
                return _string;

            _string = Convert.ToBase64String(GetBytes());
            return _string;
        }
    }
}
