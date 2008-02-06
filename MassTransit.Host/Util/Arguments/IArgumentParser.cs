using System.Collections.Generic;

namespace nu.Utility
{
    public interface IArgumentParser
    {
        IList<IArgument> Parse(string[] args);
    }
}