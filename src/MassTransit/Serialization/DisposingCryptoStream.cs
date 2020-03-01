namespace MassTransit.Serialization
{
    using System.IO;
    using System.Security.Cryptography;


    class DisposingCryptoStream :
        CryptoStream
    {
        Stream _stream;
        ICryptoTransform _transform;

        internal DisposingCryptoStream(Stream stream, ICryptoTransform transform, CryptoStreamMode mode)
            : base(stream, transform, mode)
        {
            _stream = stream;
            _transform = transform;
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            if (!HasFlushedFinalBlock)
                FlushFinalBlock();

            base.Dispose(true);

            if (_stream != null)
            {
                _stream.Dispose();
                _stream = null;
            }

            if (_transform != null)
            {
                _transform.Dispose();
                _transform = null;
            }
        }
    }
}
