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
namespace MassTransit.Host2.Hosts
{
    using System;
    using System.ServiceProcess;
    using log4net;

    public class ServiceHost : ServiceBase
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(ServiceHost));
        private HostedEnvironment _environment;
        private HostServiceInstaller _installer;


        public ServiceHost(HostedEnvironment environment)
        {
            _installer = new HostServiceInstaller(environment.ServiceName, environment.DispalyName, environment.Description);
            _environment = environment;
        }

        public void Run()
        {
            if(!_installer.IsInstalled())
            {
                _log.Fatal("Service has not been installed yet");
                throw new Exception("Service is not installed");
            }

            ServiceBase[] servicesToRun = new ServiceBase[] { this };

            ServiceBase.Run(servicesToRun);
        }

        protected override void OnStart(string[] args)
        {
            _log.Info("Received service start notification");

            _log.DebugFormat("Arguments: {0}", string.Join(",", args));
            _environment.Start();
        }

        protected override void OnStop()
        {
            _log.Info("Received service stop notification");
            _environment.Stop();
        }
    }
}