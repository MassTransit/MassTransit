namespace MassTransit.TestFramework.Courier
{
    using System.Collections.Generic;


    public interface ObjectGraphActivityArguments
    {
        OuterObject Outer { get; }
        string[] Names { get; }
        IDictionary<string, string> ArgumentsDictionary { get; }
    }
}
