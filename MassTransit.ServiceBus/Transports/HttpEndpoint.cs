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
namespace MassTransit.ServiceBus.Transports
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Net;
	using System.Runtime.Serialization.Formatters.Binary;
	using System.Threading;
	using log4net;

	/// <summary>
	/// An HttpEndpoint is designed to allow applications to participate in a remote service bus instance
	/// via HTTP instead of MSMQ. The messages are delivered through an ASHX handler within IIS to leverage
	/// the already available web serving platform and to integrate with other application concerns, such as
	/// security and logging. 
	/// 
	/// Example URI structure
	/// 
	///		http://localhost/bus/endpoint_name
	/// 
	///	When a subscription is obtained by the remote service bus that is within the network, it would
	/// publish using a mapped address:
	/// 
	///		http://localhost/bus/endpoint_name/proxy/identifier
	/// 
	/// </summary>

	public class HttpEndpoint :
		IEndpoint
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (HttpEndpoint));
		private static readonly ILog _messageLog = LogManager.GetLogger("MassTransit.Messages");

		private readonly BinaryFormatter _formatter = new BinaryFormatter();
		private readonly Uri _uri;

		public HttpEndpoint(Uri uri)
		{
			_uri = uri;
		}

		public static string Scheme
		{
			get { return "http"; }
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
			WebRequest client = HttpWebRequest.Create(_uri);
			client.Method = "PUT";
			client.Timeout = (int) timeToLive.TotalMilliseconds;
		
			using(Stream s = client.GetRequestStream())
			{
				_formatter.Serialize(s, message);
			}

			using ( WebResponse response = client.GetResponse() )
			{
				response.Close();
			}
		}

		public object Receive()
		{
			Thread.Sleep(10);

			return null;
		}

		public object Receive(TimeSpan timeout)
		{
			Thread.Sleep(timeout);

			return null;
		}

		public object Receive(Predicate<object> accept)
		{
			return Receive(TimeSpan.FromSeconds(1), accept);
		}

		public object Receive(TimeSpan timeout, Predicate<object> accept)
		{
			Thread.Sleep(timeout);

			return null;
		}

		public T Receive<T>() where T : class
		{
			return Receive<T>(TimeSpan.MaxValue);
		}

		public T Receive<T>(TimeSpan timeout) where T : class
		{
			try
			{
				return (T)Receive(timeout, delegate(object obj)
												{
													Type messageType = obj.GetType();

													if (messageType != typeof(T))
														return false;

													return true;
												});
			}
			catch (Exception ex)
			{
				string message = string.Format("Error on receive with Receive<{0}> accept", typeof(T).Name);
				_log.Error(message, ex);
			}

			throw new Exception("Receive<T>(TimeSpan timeout) didn't error");
		}

		public T Receive<T>(Predicate<T> accept) where T : class
		{
			return Receive<T>(TimeSpan.MaxValue, accept);
		}

		public T Receive<T>(TimeSpan timeout, Predicate<T> accept) where T : class
		{
			try
			{
				return (T)Receive(timeout, delegate(object obj)
												{
													Type messageType = obj.GetType();

													if (messageType != typeof(T))
														return false;

													T message = obj as T;
													if (message == null)
														return false;

													return accept(message);
												});
			}
			catch (Exception ex)
			{
				string message = string.Format("Error on receive with Predicate<{0}> accept", typeof(T).Name);
				_log.Error(message, ex);
			}

			throw new Exception("Receive<T>(TimeSpan timeout, Predicate<T> accept) had a weird error");
		}

		public void Dispose()
		{
		}
	}
}