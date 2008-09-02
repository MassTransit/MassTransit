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
namespace MassTransit.Host.Actions
{
    using System;
    using System.Reflection;
    using Configurations;
    using Hosts;
    using log4net;

    public class RunAsServiceAction :
        IAction
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (RunAsServiceAction));

        #region IAction Members

        public void Do(IInstallationConfiguration configuration)
        {
            _log.Info("Received service start notification");

            if (!HostServiceInstaller.IsInstalled(configuration))
            {
                string message = string.Format("The {0} service has not been installed yet. Please run {1} -install.",
                                               configuration.ServiceName, Assembly.GetEntryAssembly().GetName());
                _log.Fatal(message);
                throw new Exception(message);
            }

            var inServiceHost = new ServiceHost(configuration.LifeCycle);
            inServiceHost.Run();
        }

        #endregion
    }
}