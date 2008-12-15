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
namespace MassTransit.Tests
{
	using System;
	using System.Diagnostics;
	using System.Threading;

	public class FutureMessage<T>
	{
		private readonly Stopwatch _elapsed = Stopwatch.StartNew();
		private readonly ManualResetEvent _received = new ManualResetEvent(false);

		public T Message { get; private set; }

		public void Set(T message)
		{
			_elapsed.Stop();

			Trace.WriteLine("Message Received After " + _elapsed.Elapsed);

			Message = message;
			_received.Set();
		}

		public bool IsAvailable(TimeSpan timeout)
		{
			return _received.WaitOne(timeout, true);
		}
	}
}