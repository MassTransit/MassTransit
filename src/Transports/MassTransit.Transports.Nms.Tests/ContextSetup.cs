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
namespace MassTransit.Transports.Nms.Tests
{
	using System.IO;
	using System.Reflection;
	using Common.Logging;
	using NUnit.Framework;

	[SetUpFixture]
	public class ContextSetup
	{
		[SetUp]
		public void Before_any()
		{
			//string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			//string file = Path.Combine(path, "nms.test.log4net.xml");

			string file = "nms.test.log4net.xml";

			System.Collections.Specialized.NameValueCollection properties = new System.Collections.Specialized.NameValueCollection();
			properties.Add("configType", "File");
			properties.Add("configFile", file);
			Common.Logging.LogManager.Adapter = new Common.Logging.Log4Net.Log4NetLoggerFactoryAdapter(properties);
		}
	}
}