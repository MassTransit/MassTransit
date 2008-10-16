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
    using System;
    using System.Configuration;
    using System.ServiceProcess;

    public class DotNetConfiguration :
        BaseWinServiceConfiguration
    {
        public override IApplicationLifeCycle LifeCycle
        {
            get { throw new NotImplementedException(); }
        }

        public override Credentials Credentials
        {
            get
            {
                return new Credentials(
                    ConfigurationManager.AppSettings["username"],
                    ConfigurationManager.AppSettings["password"]);
            }
        }

        public override string ServiceName
        {
            get { return ConfigurationManager.AppSettings["serviceName"]; }
        }

        public override string DisplayName
        {
            get { return ConfigurationManager.AppSettings["displayName"]; }
        }

        public override string Description
        {
            get { return ConfigurationManager.AppSettings["description"]; }
        }

        public override string[] Dependencies
        {
            get { return ConfigurationManager.AppSettings["dependencies"].Split(','); }
        }

        public override void ConfigureServiceProcessInstaller(ServiceProcessInstaller installer)
        {
            installer.Account = ServiceAccount.User;
            installer.Username = Credentials.Username;
            installer.Password = Credentials.Password;
        }
    }
}