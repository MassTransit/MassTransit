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
namespace Grid.Distributor.Worker
{
	using System;
	using System.Configuration;
    using System.IO;
    using log4net.Config;

    internal class Program
    {
        private static void Main()
        {
            XmlConfigurator.ConfigureAndWatch(new FileInfo(@"worker.log4net.config"));

        	try
        	{
        		var service = new WorkerServiceProvider
        			{
        				ServiceName = ConfigurationManager.AppSettings["ServiceName"],
        				DisplayName = ConfigurationManager.AppSettings["DisplayName"],
        				Description = ConfigurationManager.AppSettings["Description"],
        				SourceQueue = ConfigurationManager.AppSettings["SourceQueue"],
        			};

        		service.ConfigureService<DoWork>();
        	}
        	catch (Exception ex)
        	{
        		Console.WriteLine(ex);
        	}
        }
    }
}