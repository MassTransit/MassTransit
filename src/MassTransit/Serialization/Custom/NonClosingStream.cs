namespace MassTransit.Serialization.Custom
{
	using System.IO;

	/// <summary>
	/// A stream that doesn't close the inner stream when <see cref="Close"/> is called.
	/// </summary>
	public class NonClosingStream :
		Stream
	{
		private readonly Stream _stream;

		public NonClosingStream(Stream stream)
		{
			_stream = stream;
		}

		public override bool CanRead
		{
			get { return _stream.CanRead; }
		}

		public override bool CanSeek
		{
			get { return _stream.CanSeek; }
		}

		public override bool CanWrite
		{
			get { return _stream.CanWrite; }
		}

		public override long Length
		{
			get { return _stream.Length; }
		}

		public override long Position
		{
			get { return _stream.Position; }
			set { _stream.Position = value; }
		}

		public override void Flush()
		{
			_stream.Flush();
		}

		public override void Close()
		{
		}

		protected override void Dispose(bool disposing)
		{
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			return _stream.Seek(offset, origin);
		}

		public override void SetLength(long value)
		{
			_stream.SetLength(value);
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			return _stream.Read(buffer, offset, count);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			_stream.Write(buffer, offset, count);
		}
	}
}