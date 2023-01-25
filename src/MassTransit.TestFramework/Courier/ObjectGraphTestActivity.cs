namespace MassTransit.TestFramework.Courier
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;


    public class ObjectGraphTestActivity :
        IActivity<ObjectGraphActivityArguments, TestLog>
    {
        readonly IDictionary<string, string> _argumentsDictionary;
        readonly decimal _decimalValue;
        readonly int _intValue;
        readonly string[] _names;
        readonly string _stringValue;

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
            var intValue = context.Arguments.Outer.IntValue;
            var stringValue = context.Arguments.Outer.StringValue;
            var decimalValue = context.Arguments.Outer.DecimalValue;
            string[] names = context.Arguments.Names;
            IDictionary<string, string> argumentsDictionary = context.Arguments.ArgumentsDictionary;

            Console.WriteLine("TestActivity: Execute: {0}, {1}, {2}, [{3}],[{4}]", intValue, stringValue, decimalValue,
                string.Join(",", names), string.Join(",", argumentsDictionary.Select(x => $"{x.Key} => {x.Value}")));

            if (_intValue != intValue)
                throw new ArgumentException("intValue");
            if (_stringValue != stringValue)
                throw new ArgumentException("stringValue");
            if (_decimalValue != decimalValue)
                throw new ArgumentException("dateTimeValue");

            return context.Completed<TestLog>(new {OriginalValue = stringValue});
        }

        public async Task<CompensationResult> Compensate(CompensateContext<TestLog> context)
        {
            Console.WriteLine("TestActivity: Compensate original value: {0}", context.Log.OriginalValue);

            return context.Compensated();
        }
    }
}
