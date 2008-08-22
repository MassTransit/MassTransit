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
namespace MassTransit.Grid.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    public class FactorLongNumberWorker :
        ISubTaskWorker<FactorLongNumber, LongNumberFactored>
    {
        private readonly IntegerFactorizer _factorizer = new IntegerFactorizer();

        public void ExecuteTask(FactorLongNumber task, Action<LongNumberFactored> result)
        {
            IList<long> factors = _factorizer.Factor(task.Value);

            Debug.WriteLine(string.Format("Factored {0} into {1}", task.Value, factors.Join(",")));

            LongNumberFactored longNumberFactored = new LongNumberFactored(factors);

            result(longNumberFactored);
        }
    }
}