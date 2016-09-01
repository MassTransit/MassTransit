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
namespace MassTransit.TestFramework.Courier
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using MassTransit.Courier;


    public class ObjectGraphTestActivity :
        Activity<ObjectGraphActivityArguments, TestLog>
    {
        readonly decimal _decimalValue;
        readonly int _intValue;
        readonly string[] _names;
        readonly string _stringValue;
        readonly IDictionary<string, string> _argumentsDictionary;

        public ObjectGraphTestActivity(int intValue, string stringValue, decimal decimalValue, string[] names, IDictionary<string, string> argumentsDictionary)
        {
            _intValue = intValue;
            _stringValue = stringValue;
            _decimalValue = decimalValue;
            _names = names;
            _argumentsDictionary = argumentsDictionary;
        }
        public async Task<ExecutionResult> Execute(ExecuteContext<ObjectGraphActivityArguments> context)
        {
            int intValue = context.Arguments.Outer.IntValue;
            string stringValue = context.Arguments.Outer.StringValue;
            decimal decimalValue = context.Arguments.Outer.DecimalValue;
            string[] names = context.Arguments.Names;
            IDictionary<string, string> argumentsDictionary = context.Arguments.ArgumentsDictionary;

            Console.WriteLine("TestActivity: Execute: {0}, {1}, {2}, [{3}],[{4}]", intValue, stringValue, decimalValue,
                          string.Join(",", names), string.Join(",", argumentsDictionary.Keys));

            if (_intValue != intValue)
                throw new ArgumentException("intValue");
            if (_stringValue != stringValue)
                throw new ArgumentException("stringValue");
            if (_decimalValue != decimalValue)
                throw new ArgumentException("dateTimeValue");

            TestLog log = new TestLogImpl(stringValue);

            return context.Completed(log);
        }

        public async Task<CompensationResult> Compensate(CompensateContext<TestLog> context)
        {
            Console.WriteLine("TestActivity: Compensate original value: {0}", context.Log.OriginalValue);

            return context.Compensated();
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