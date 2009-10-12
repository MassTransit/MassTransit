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
namespace MassTransit.Monitoring
{
	using System;
	using System.Diagnostics;

	public class InstancePerformanceCounter :
		IPerformanceCounter
	{
		private bool _disposed;
		private PerformanceCounter _pcs;

		public InstancePerformanceCounter(string name, string categoryName, string instanceName)
		{
			_pcs = new PerformanceCounter(categoryName, name, instanceName, false)
				{
					RawValue = 0
				};
		}

		public string Name
		{
			get { return _pcs.CounterName; }
		}

		public string InstanceName
		{
			get { return _pcs.InstanceName; }
		}

		public string CategoryName
		{
			get { return _pcs.CategoryName; }
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void Close()
		{
			if (_pcs != null)
			{
				_pcs.RemoveInstance();
				_pcs.Close();
				_pcs = null;
			}
		}

		public virtual void Increment()
		{
			_pcs.Increment();
		}

		public virtual void IncrementBy(long val)
		{
			_pcs.IncrementBy(val);
		}

		public virtual void Set(long val)
		{
			_pcs.RawValue = val;
		}

		private void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				Close();
			}

			_disposed = true;
		}

		~InstancePerformanceCounter()
		{
			Dispose(false);
		}
	}
}