// Copyright 2007-2013 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Transports.Msmq.Management
{
    using System;
    using System.Diagnostics;
    using System.IO;

    public class WindowsServer2012Installer :
        MsmqInstaller
    {
        const string PSCommand = "Add-WindowsFeature MSMQ-Services,MSMQ-Server,MSMQ-Multicasting";
        const string Command = "-command " + PSCommand;
        const string PowerShell = @"WindowsPowerShell\v1.0\powershell.exe";

        public Process Install()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.System);
            if (IntPtr.Size == 4)
                path = path.Replace("system32", "SysNative");
            string file = Path.Combine(path, PowerShell);

            return Process.Start(file, Command);
        }
    }
}
