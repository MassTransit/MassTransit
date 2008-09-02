namespace MassTransit.Host
{
    using System.Collections.Generic;
    using MassTransitHost.ArgumentParsing;

    public class Parser
    {
        public static Args ParseArgs(string[] args)
        {
            if(args == null) args = new string[0];

            Args result = new Args();
            IArgumentMapFactory _argumentMapFactory = new ArgumentMapFactory();
            IArgumentParser _argumentParser = new ArgumentParser();
            IEnumerable<IArgument> arguments = _argumentParser.Parse(args);
            IArgumentMap mapper = _argumentMapFactory.CreateMap(result);
            IEnumerable<IArgument> remaining = mapper.ApplyTo(result, arguments);

            return result;
        }
        public class Args
        {
            private bool _install;
            private bool _uninstall;
            private bool _console;
            private bool _service;


            [Argument(Key = "install")]
            public bool Install
            {
                get { return _install; }
                set { _install = value; }
            }

            [Argument(Key = "uninstall")]
            public bool Uninstall
            {
                get { return _uninstall; }
                set { _uninstall = value; }
            }

            [Argument(Key = "console")]
            public bool Console
            {
                get { return _console; }
                set { _console = value; }
            }

            [Argument(Key = "service")]
            public bool Service
            {
                get { return _service; }
                set { _service = value; }
            }


            public string GetActionKey()
            {
                if (Install) return "install";
                if (Uninstall) return "uninstall";
                if (Service) return "service";
                return "console";
            }
        }
        
    }
}