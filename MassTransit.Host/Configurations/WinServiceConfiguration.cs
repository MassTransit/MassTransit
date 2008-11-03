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
namespace MassTransit.Host.Configurations
{
    using System.ServiceProcess;

    public class WinServiceConfiguration :
        IInstallationConfiguration
    {
        private readonly Credentials _credentials;
        private readonly WinServiceSettings _settings;
        private readonly IApplicationLifecycle _lifecycle;

        public WinServiceConfiguration(Credentials credentials, WinServiceSettings settings, IApplicationLifecycle lifecycle)
        {
            _credentials = credentials;
            _settings = settings;
            _lifecycle = lifecycle;
        }

        public Credentials Credentials
        {
            get { return _credentials; }
        }

        public WinServiceSettings Settings
        {
            get { return _settings; }
        }

        public IApplicationLifecycle Lifecycle
        {
            get { return _lifecycle; }
        }

        public virtual void ConfigureServiceInstaller(ServiceInstaller installer)
        {
            installer.ServiceName = Settings.ServiceName;
            installer.Description = Settings.Description;
            installer.DisplayName = Settings.DisplayName;
            installer.ServicesDependedOn = Settings.Dependencies;
            installer.StartType = ServiceStartMode.Automatic;
        }
        public virtual void ConfigureServiceProcessInstaller(ServiceProcessInstaller installer)
        {
            installer.Username = Credentials.Username;
            installer.Password = Credentials.Password;
            installer.Account = Credentials.AccountType;
        }
    }
}