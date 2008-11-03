// Copyright 2007-2008 The Apache Software Foundation.
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
namespace MassTransit.Host
{
    using System.Collections.Generic;
	using Actions;
	using log4net;
    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// Entry point into the Host infrastructure
    /// </summary>
	public static class Runner
	{
		private static readonly IDictionary<NamedAction, IAction> _actions = new Dictionary<NamedAction, IAction>();
		private static readonly ILog _log = LogManager.GetLogger(typeof (Runner));

		static Runner()
		{
			_actions.Add(ServiceNamedAction.Install, new InstallServiceAction());
			_actions.Add(ServiceNamedAction.Uninstall, new UninstallServiceAction());
			_actions.Add(NamedAction.Console, new RunAsConsoleAction());
			_actions.Add(NamedAction.Gui, new RunAsWinFormAction());
			_actions.Add(ServiceNamedAction.Service, new RunAsServiceAction());
		}

        /// <summary>
        /// Go go gadget
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="args"></param>
        public static void Run(IInstallationConfiguration configuration, params string[] args)
        {
            _log.Info("Starting Host");
            _log.DebugFormat("Arguments: {0}", string.Join(",", args));

            NamedAction actionKey;
            var arguments = Parser.ParseArgs(args);

            if (arguments.IsDefault)
            {
                actionKey = configuration.Lifecycle.DefaultAction;
            }
            else
            {
                actionKey = arguments.GetActionKey();
            }

            IAction action = _actions[actionKey];
            _log.DebugFormat("Running action: {0}", action);
            action.Do(configuration, ServiceLocator.Current);
        }
	}
}