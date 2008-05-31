/// Copyright 2007-2008 The Apache Software Foundation.
/// 
/// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
/// this file except in compliance with the License. You may obtain a copy of the 
/// License at 
/// 
///   http://www.apache.org/licenses/LICENSE-2.0 
/// 
/// Unless required by applicable law or agreed to in writing, software distributed 
/// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
/// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
/// specific language governing permissions and limitations under the License.
namespace MassTransit.ServiceBus.NMS
{
	using System;

	public class NmsEndpoint :
		INmsEndpoint
	{
		private Uri _uri;

		public NmsEndpoint(Uri uri)
		{
			_uri = uri;
		}

		public NmsEndpoint(string uriString)
		{
			_uri = new Uri(uriString);
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
		}
	}
}