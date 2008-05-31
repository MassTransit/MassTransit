namespace MassTransit.ServiceBus.Tests
{
	using System;
	using Internal;
	using NUnit.Framework;

	[TestFixture]
	public class As_an_EndpointResolver
	{
		[SetUp]
		public void I_want_to()
		{
		}

		[Test]
		public void Be_able_to_resolve_endpoints()
		{
			EndpointResolver res = new EndpointResolver();
			res.Initialize();
			IEndpoint ep = res.Resolve(new Uri("msmq://localhost/test"));
			Assert.IsNotNull(ep);
		}

		[Test]
		public void Be_intializable()
		{
			EndpointResolver res = new EndpointResolver();
			res.Initialize();
		}
	}

	public class FakeEndpoint : IEndpoint
	{
		private Uri _uri;

		public FakeEndpoint(Uri uri)
		{
			_uri = uri;
		}


		public Uri Uri
		{
			get { return _uri; }
		}

		public void Send<T>(T message) where T : class
		{
			throw new NotImplementedException();
		}

		public void Send<T>(T message, TimeSpan timeToLive) where T : class
		{
			throw new NotImplementedException();
		}

		public object Receive()
		{
			throw new NotImplementedException();
		}

		public object Receive(TimeSpan timeout)
		{
			throw new NotImplementedException();
		}

		public object Receive(Predicate<object> accept)
		{
			throw new NotImplementedException();
		}

		public object Receive(TimeSpan timeout, Predicate<object> accept)
		{
			throw new NotImplementedException();
		}

		public T Receive<T>() where T : class
		{
			throw new NotImplementedException();
		}

		public T Receive<T>(TimeSpan timeout) where T : class
		{
			throw new NotImplementedException();
		}

		public T Receive<T>(Predicate<T> accept) where T : class
		{
			throw new NotImplementedException();
		}

		public T Receive<T>(TimeSpan timeout, Predicate<T> accept) where T : class
		{
			throw new NotImplementedException();
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}
	}
}