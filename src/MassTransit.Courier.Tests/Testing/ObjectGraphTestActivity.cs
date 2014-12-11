// Copyright 2007-2013 Chris Patterson
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
namespace MassTransit.Courier.Tests.Testing
{
    using System;
    using System.Threading.Tasks;


    public class ObjectGraphTestActivity :
        Activity<ObjectGraphActivityArguments, TestLog>
    {
        readonly decimal _decimalValue;
        readonly int _intValue;
        readonly string[] _names;
        readonly string _stringValue;

        public ObjectGraphTestActivity(int intValue, string stringValue, decimal decimalValue, string[] names)
        {
            _intValue = intValue;
            _stringValue = stringValue;
            _decimalValue = decimalValue;
            _names = names;
        }

        public async Task<ExecutionResult> Execute(Execution<ObjectGraphActivityArguments> execution)
        {
            int intValue = execution.Arguments.Outer.IntValue;
            string stringValue = execution.Arguments.Outer.StringValue;
            decimal decimalValue = execution.Arguments.Outer.DecimalValue;
            string[] names = execution.Arguments.Names;

            Console.WriteLine("TestActivity: Execute: {0}, {1}, {2}, [{3}]", intValue, stringValue, decimalValue,
                string.Join(",", names));

            if (_intValue != intValue)
                throw new ArgumentException("intValue");
            if (_stringValue != stringValue)
                throw new ArgumentException("stringValue");
            if (_decimalValue != decimalValue)
                throw new ArgumentException("dateTimeValue");

            TestLog log = new TestLogImpl(stringValue);

            return execution.Completed(log);
        }

        public async Task<CompensationResult> Compensate(Compensation<TestLog> compensation)
        {
            Console.WriteLine("TestActivity: Compensate original value: {0}", compensation.Log.OriginalValue);

            return compensation.Compensated();
        }


        class TestLogImpl :
            TestLog
        {
            public TestLogImpl(string originalValue)
            {
                OriginalValue = originalValue;
            }

            public string OriginalValue { get; private set; }
        }
    }
}