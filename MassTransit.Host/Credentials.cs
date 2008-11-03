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
    using System;
    using System.Configuration;
    using System.ServiceProcess;

    public class Credentials : IEquatable<Credentials>
    {
        private readonly string _username;
        private readonly string _password;
        private readonly ServiceAccount _accountType;

        public Credentials(string username, string password, ServiceAccount accountType)
        {
            _username = username;
            _accountType = accountType;
            _password = password;
        }

        public string Username
        {
            get { return _username; }
        }

        public string Password
        {
            get { return _password; }
        }

        public ServiceAccount AccountType
        {
            get { return _accountType; }
        }

        public static Credentials LocalSystem
        {
            get
            {
                return new Credentials("","", ServiceAccount.LocalSystem);
            }
        }

        public static Credentials Interactive
        {
            get
            {
                return new Credentials(null,null,ServiceAccount.User);
            }
        }

        public static Credentials DotNetConfig
        {
            get
            {
                return new Credentials(ConfigurationManager.AppSettings["username"],
                                       ConfigurationManager.AppSettings["password"],
                                       ServiceAccount.User);
            }
        }

        #region System.String Overrides

        public bool Equals(Credentials obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj._username, _username) && Equals(obj._password, _password);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Credentials)) return false;
            return Equals((Credentials) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_username != null ? _username.GetHashCode() : 0)*397) ^ (_password != null ? _password.GetHashCode() : 0);
            }
        }

        #endregion
    }
}