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
namespace MassTransit.Threading
{
	using System;
	using System.Threading;

	public class ManagedThread :
		IDisposable
	{
		private readonly TimeSpan _threadExitWaitTime = TimeSpan.FromMinutes(1);
		private volatile bool _disposed;
		private ManualResetEvent _shutdown = new ManualResetEvent(false);
		private Thread _thread;

		public ManagedThread()
		{
			_thread = new Thread(RunThread);
		}

		protected ManualResetEvent Shutdown
		{
			get { return _shutdown; }
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				Stop();

				_thread = null;

				_shutdown.Close();
				_shutdown = null;
			}

			_disposed = true;
		}

		~ManagedThread()
		{
			Dispose(false);
		}

		public bool Start()
		{
			if (_thread.IsAlive)
				return false;

			_shutdown.Reset();
			_thread.Start();

			return true;
		}

		public void Start(object obj)
		{
			if (_thread.IsAlive)
				return;

			_shutdown.Reset();
			_thread.Start(obj);
		}

		public void Stop()
		{
			if (_thread.IsAlive)
			{
				_shutdown.Set();
				_thread.Join(_threadExitWaitTime);
			}
		}

		protected virtual void RunThread(object obj)
		{
			WaitHandle[] handles = new WaitHandle[] {_shutdown};

			int result;
			while ((result = WaitHandle.WaitAny(handles, TimeSpan.FromSeconds(5), true)) != 0)
			{
			}
		}
	}
}