// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Monitoring.Performance
{
    using System;
    using System.Diagnostics;


    public class InstancePerformanceCounter :
        IPerformanceCounter
    {
        PerformanceCounter _counter;

        public InstancePerformanceCounter(PerformanceCounter counter)
        {
            _counter = counter;
        }

        public void Dispose()
        {
            if (_counter != null)
            {
                try
                {
                    _counter.RemoveInstance();
                    _counter.Close();
                }
                catch (NotImplementedException)
                {
                    // blame mono for this.
                }
                finally
                {
                    _counter.Dispose();
                    _counter = null;
                }
            }
        }

        public void Increment()
        {
            _counter.Increment();
        }

        public void IncrementBy(long val)
        {
            _counter.IncrementBy(val);
        }

        public void Set(long val)
        {
            _counter.RawValue = val;
        }
    }
}