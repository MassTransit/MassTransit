/// Copyright 2007-2008 The Apache Software Foundation.
/// 
/// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
/// this file except in compliance with the License. You may obtain a copy of the 
/// License at 
/// 
///   http://www.apache.org/licenses/LICENSE-2.0 
/// 
/// Unless required by applicable law or agreed to in writing, software distributed 
/// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
/// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
/// specific language governing permissions and limitations under the License.
namespace MassTransit.Host
{
    using System.Collections.Generic;
    using Actions;
    using log4net;
    using MassTransitHost.ArgumentParsing;

    public static class Runner
    {
        private static ILog _log = LogManager.GetLogger(typeof (Runner));
        private static IDictionary<string, IAction> _actions = new Dictionary<string, IAction>();

        static Runner()
        {
            _actions.Add("install", new InstallServiceAction());
            _actions.Add("uninstall", new UninstallServiceAction());
            _actions.Add("console", new RunAsConsoleAction());
            _actions.Add("service", new RunAsServiceAction());
        }

        public static void Run(HostedEnvironment environment, params string[] args)
        {
            _log.Info("Starting Host");
            _log.DebugFormat("Arguments: {0}", string.Join(",", args));

            //TODO: Hacky
            string action = ParseArgs(args).GetAction();
            _log.DebugFormat("Running action: {0}", action);

            _actions[action].Do(environment);
        } 

        

        public static RunnerArgs ParseArgs(string[] args)
        {
            RunnerArgs result = new RunnerArgs();
            IArgumentMapFactory _argumentMapFactory = new ArgumentMapFactory();
            IArgumentParser _argumentParser = new ArgumentParser();
            IEnumerable<IArgument> arguments = _argumentParser.Parse(args);
            IArgumentMap mapper = _argumentMapFactory.CreateMap(result);
            IEnumerable<IArgument> remaining = mapper.ApplyTo(result, arguments);

            return result;
        }

        public class RunnerArgs
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


            public string GetAction()
            {
                if (Install) return "install";
                if (Uninstall) return "uninstall";
                if (Service) return "service";
                return "console";
            }
        }
    }
}