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
    using System;
    using System.Collections;
    using System.Configuration.Install;
    using System.Reflection;
    using System.ServiceProcess;
    using Configurations;
    using log4net;
    using Microsoft.Win32;

    public class HostServiceInstaller :
        Installer
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (HostServiceInstaller));
        private readonly ServiceInstaller _serviceInstaller = new ServiceInstaller();
        private readonly ServiceProcessInstaller _serviceProcessInstaller = new ServiceProcessInstaller();
        private readonly IInstallationConfiguration _config;

        public HostServiceInstaller(IInstallationConfiguration configuration)
        {
            _log.DebugFormat("Attempting to install with {0} configuration", configuration);
            _config = configuration;

            configuration.ConfigureServiceInstaller(_serviceInstaller);
            configuration.ConfigureServiceProcessInstaller(_serviceProcessInstaller);

            Installers.AddRange(new Installer[] {_serviceProcessInstaller, _serviceInstaller});            
        }


        public void Register()
        {
            if (!IsInstalled(_config))
            {
                using (TransactedInstaller ti = new TransactedInstaller())
                {
                    ti.Installers.Add(this);

                    string path = string.Format("/assemblypath={0}", Assembly.GetEntryAssembly().Location);
                    string[] commandLine = {path};

                    InstallContext context = new InstallContext(null, commandLine);
                    ti.Context = context;

                    Hashtable savedState = new Hashtable();

                    ti.Install(savedState);
                }
            }
            else
            {
                Console.WriteLine("Service is already installed");
                if (_log.IsInfoEnabled)
                    _log.Info("Service is already installed");
            }
        }
        public void Unregister()
        {
            if (IsInstalled(_config))
            {
                using (TransactedInstaller ti = new TransactedInstaller())
                {
                    ti.Installers.Add(this);

                    string path = string.Format("/assemblypath={0}", Assembly.GetEntryAssembly().Location);
                    string[] commandLine = {path};

                    InstallContext context = new InstallContext(null, commandLine);
                    ti.Context = context;

                    ti.Uninstall(null);
                }
            }
            else
            {
                Console.WriteLine("Service is not installed");
                if (_log.IsInfoEnabled)
                    _log.Info("Service is not installed");
            }
        }

        public static bool IsInstalled(IInstallationConfiguration configuration)
        {
            foreach (ServiceController service in ServiceController.GetServices())
            {
                if (service.ServiceName == configuration.ServiceName)
                    return true;
            }

            return false;
        }


        /// <summary>
        /// For the .Net service install infrastructure
        /// </summary>
        /// <param name="stateSaver"></param>
        public override void Install(IDictionary stateSaver)
        {
            if (_log.IsInfoEnabled)
                _log.InfoFormat("Installing Service {0}", _serviceInstaller.ServiceName);

            base.Install(stateSaver);

            if (_log.IsInfoEnabled)
                _log.InfoFormat("Opening Registry");

            using (RegistryKey system = Registry.LocalMachine.OpenSubKey("System"))
            using (RegistryKey currentControlSet = system.OpenSubKey("CurrentControlSet"))
            using (RegistryKey services = currentControlSet.OpenSubKey("Services"))
            using (RegistryKey service = services.OpenSubKey(_serviceInstaller.ServiceName, true))
            {
                service.SetValue("Description", _serviceInstaller.Description);

                string imagePath = (string)service.GetValue("ImagePath");

                _log.InfoFormat("Service Path {0}", imagePath);

                imagePath += " -service";

                service.SetValue("ImagePath", imagePath);


                service.Close();
                services.Close();
                currentControlSet.Close();
                system.Close();
            }
        }
    }
}