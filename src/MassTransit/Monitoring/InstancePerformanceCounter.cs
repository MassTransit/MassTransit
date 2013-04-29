// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
        bool _disposed;
        PerformanceCounter _performanceCounter;

        public InstancePerformanceCounter(string name, string categoryName, string instanceName)
        {
            _performanceCounter = new PerformanceCounter(categoryName, name, instanceName, false)
                {
                    RawValue = 0
                };
        }

        public string Name
        {
            get { return _performanceCounter.CounterName; }
        }

        public string InstanceName
        {
            get { return _performanceCounter.InstanceName; }
        }

        public string CategoryName
        {
            get { return _performanceCounter.CategoryName; }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public void Close()
        {
            if (_performanceCounter != null)
            {
                try
                {
                    _performanceCounter.RemoveInstance();
                    _performanceCounter.Close();
                }
                catch (NotImplementedException)
                {
                    // blame mono for this.
                }
                finally
                {
                    _performanceCounter = null;
                }
            }
        }

        public virtual void Increment()
        {
            _performanceCounter.Increment();
        }

        public virtual void IncrementBy(long val)
        {
            _performanceCounter.IncrementBy(val);
        }

        public virtual void Set(long val)
        {
            _performanceCounter.RawValue = val;
        }

        void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                Close();
            }

            _disposed = true;
        }
    }
}