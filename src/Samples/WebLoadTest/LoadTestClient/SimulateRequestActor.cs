// Copyright 2007-2008 The Apache Software Foundation.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace LoadTestClient
{
	using System;
	using System.IO;
	using System.Net;
	using System.Text;
	using System.Threading;
	using Magnum.Actors;
	using Magnum.Actors.CommandQueues;

	public class SimulateRequestActor
	{
		private static long _requestCount;
		private readonly CommandQueue _queue = new ThreadPoolCommandQueue();
		private readonly Uri _targetUri;
		private static int _activeRequests;

		public SimulateRequestActor(Uri targetUri)
		{
			_targetUri = targetUri;

			_queue.Enqueue(MakeRequest);
		}

		public static long RequestCount
		{
			get { return _requestCount; }
		}

		public void Stop()
		{
			_queue.Disable();
		}

		private void MakeRequest()
		{
			try
			{
				var request = (HttpWebRequest) WebRequest.Create(_targetUri);
				request.ProtocolVersion = HttpVersion.Version10;
				Interlocked.Increment(ref _activeRequests);
				request.BeginGetResponse(asyncResult =>
					{
						try
						{
							WebResponse response = request.EndGetResponse(asyncResult);

							long value = Interlocked.Increment(ref _requestCount);
							if (value%100 == 0)
								Console.WriteLine("Active: " + _activeRequests + ", Total: " + _requestCount);

							var content = new MemoryStream();

							const int blockSize = 16384;
							var block = new byte[blockSize];

							Stream responseStream = response.GetResponseStream();

							Func<IAsyncResult> readChunk = null;

							readChunk = () => responseStream.BeginRead(block, 0, blockSize, result =>
								{
									int read = responseStream.EndRead(result);
									if (read > 0)
									{
										content.Write(block, 0, read);

										readChunk();
									}
									else
									{
										try
										{
											if (value%1000 == 0)
											{
												Console.WriteLine(Encoding.UTF8.GetString(content.ToArray()));
											}

											content.Dispose();

											Interlocked.Decrement(ref _activeRequests);
											responseStream.Dispose();
											using (response)
											{
											}
										}
										catch (Exception ex)
										{
											Console.WriteLine(ex.Message);
										}
										finally
										{
											_queue.Enqueue(MakeRequest);
										}
									}
								}, null);
							readChunk();
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.Message);
							_queue.Enqueue(MakeRequest);
						}
					}, null);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}
	}
}