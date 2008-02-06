namespace nu.Utility
{
    using System.Collections.Generic;

    public class ArgumentParser : IArgumentParser
    {
        private static readonly char[] _switchChars = new char[] {':', '+', '-'};

        #region IArgumentParser Members

        public IList<IArgument> Parse(string[] args)
        {
            List<IArgument> arguments = new List<IArgument>();

            foreach (string arg in args)
            {
                if (arg.Length == 0)
                    continue;

                switch (arg[0])
                {
                    case '-':
                    case '/':

                        int end = arg.IndexOfAny(_switchChars, 1);
                        if (end == -1)
                        {
                            Argument switchArg = new Argument(arg.Substring(1), "true");
                            arguments.Add(switchArg);
                        }
                        else
                        {
                            string key = arg.Substring(1, end - 1);
                            string value = "true";

                            if (arg[end] == '-')
                                value = "false";
                            else if (arg[end] == ':')
                            {
                                if (arg.Length > key.Length + 2)
                                    value = arg.Substring(key.Length + 2);
                                else
                                    value = string.Empty;
                            }

                            Argument switchArg = new Argument(key, value);
                            arguments.Add(switchArg);
                        }
                        break;

                    default:
                        Argument argument = new Argument(null, arg);
                        arguments.Add(argument);
                        break;
                }
            }

            return arguments;
        }

        #endregion
    }
}