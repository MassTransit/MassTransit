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
    using Configurations;
    using log4net;

    public static class Runner
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (Runner));
        private static readonly IDictionary<string, IAction> _actions = new Dictionary<string, IAction>();

        static Runner()
        {
            _actions.Add("install", new InstallServiceAction());
            _actions.Add("uninstall", new UninstallServiceAction());
            _actions.Add("console", new RunAsConsoleAction());
            _actions.Add("service", new RunAsServiceAction());
        }

        public static void Run(IInstallationConfiguration environment, params string[] args)
        {
            _log.Info("Starting Host");
            _log.DebugFormat("Arguments: {0}", string.Join(",", args));

            //TODO: Hacky
            string actionKey = Parser.ParseArgs(args).GetActionKey();
            _log.DebugFormat("Running action: {0}", actionKey);

            _actions[actionKey].Do(environment);
        }
    }
}