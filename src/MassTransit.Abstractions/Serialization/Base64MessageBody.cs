namespace MassTransit
{
    using System;
    using System.IO;


    /// <summary>
    /// Converts a binary-only message body to Base64 so that it can be used with non-binary message transports.
    /// Only the <see cref="GetString" /> method performs the conversion, <see cref="GetBytes" /> and <see cref="GetStream" />
    /// are pass-through methods.
    /// </summary>
    public class Base64MessageBody :
        MessageBody
    {
        readonly string _text;
        byte[]? _bytes;

        public Base64MessageBody(string text)
        {
            _text = text;
        }

        public long? Length => _text.Length;

        public Stream GetStream()
        {
            return new MemoryStream(GetBytes(), false);
        }

        public byte[] GetBytes()
        {
            if (_bytes != null)
                return _bytes;

            _bytes = Convert.FromBase64String(_text);

            return _bytes;
        }

        public string GetString()
        {
            return _text;
        }
    }
}
