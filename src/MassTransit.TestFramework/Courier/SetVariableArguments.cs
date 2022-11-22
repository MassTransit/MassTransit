namespace MassTransit.TestFramework.Courier
{
    using System;


    public interface SetVariableArguments
    {
        string Key { get; }
        string Value { get; }

        Guid GuidValue { get; }
    }
}
