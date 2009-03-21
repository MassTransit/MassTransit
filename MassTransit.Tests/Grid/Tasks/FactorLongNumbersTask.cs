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
namespace MassTransit.Tests.Grid
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Magnum;
    using MassTransit.Grid;
    using Util;

    public class FactorLongNumbersTask :
        IDistributedTask<FactorLongNumbersTask, FactorLongNumber, LongNumberFactored>
    {
        private readonly Dictionary<int, IList<long>> _results = new Dictionary<int, IList<long>>();
        private readonly List<long> _values = new List<long>();
        private Action<FactorLongNumbersTask> _completed = delegate { };
        private Action<FactorLongNumbersTask, long, Exception> _exceptionOccurred = delegate { };

        public int SubTaskCount
        {
            [DebuggerStepThrough]
            get { return _values.Count; }
        }

        public FactorLongNumber GetSubTaskInput(int subTaskId)
        {
            Guard.Against.IndexOutOfRange(subTaskId, SubTaskCount);

            return new FactorLongNumber(_values[subTaskId]);
        }

        public void DeliverSubTaskOutput(int subTaskId, LongNumberFactored output)
        {
            lock (_results)
            {
                if (_results.ContainsKey(subTaskId))
                    _results[subTaskId] = output.Factors;
                else
                    _results.Add(subTaskId, output.Factors);
            }

            if (_values.Count == _results.Count)
                _completed(this);
        }

        public void NotifySubTaskException(int subTaskId, Exception ex)
        {
            _exceptionOccurred(this, subTaskId, ex);
        }

        public void WhenCompleted(Action<FactorLongNumbersTask> action)
        {
            _completed += action;
        }

        public void Add(long value)
        {
            _values.Add(value);
        }

        public void WhenExceptionOccurs(Action<FactorLongNumbersTask, long, Exception> action)
        {
            _exceptionOccurred += action;
        }
    }
}