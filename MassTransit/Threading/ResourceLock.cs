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

	public class ResourceLock<T>
	{
		private readonly T _resource;
		private readonly Semaphore _semaphore;
		private readonly ManualResetEvent _shutdown = new ManualResetEvent(false);
		private readonly WaitHandle[] _waitHandles;

		public ResourceLock(T resource)
			: this(resource, 1)
		{
		}

		public ResourceLock(T resource, int limit)
		{
			_resource = resource;
			_semaphore = new Semaphore(limit, limit);
			_waitHandles = new WaitHandle[] {_shutdown, _semaphore};
		}

		public void Shutdown()
		{
			_shutdown.Set();
		}

		public IResourceLockScope<T> AcquireLock(TimeSpan timeout)
		{
			int result = WaitHandle.WaitAny(_waitHandles, timeout, true);
			if (result == 0)
				throw new ResourceLockException("The resource is being shut down and cannot be locked");

			if (result == 1)
				return new ResourceLockScope<T>(_resource, () => _semaphore.Release());

			if (result == WaitHandle.WaitTimeout)
				throw new ResourceLockException("A lock could not be acquired within the timeout period");

			throw new ResourceLockException("Some unknown reason prevented the lock from being acquired");
		}
	}
}