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
namespace MassTransit.TestFramework.Fixtures
{
	using System;
	using System.Diagnostics;
	using log4net;
	using Magnum.ObjectExtensions;
	using NUnit.Framework;

	[TestFixture]
	public class AbstractTestFixture
	{
		[SetUp]
		public void AbstractSetup()
		{
			_timer = Stopwatch.StartNew();
		}

		[TestFixtureSetUp]
		public void AbstractTestFixtureSetup()
		{
			OutputTestName(GetType());
		}

		[TestFixtureTearDown]
		public void AbstractTestFixtureTeardown()
		{
			_timer.Stop();

			Log.InfoFormat("Elapsed Time: {0}ms", _timer.ElapsedMilliseconds);
			Log.Info("");
		}

		private Stopwatch _timer;

		protected static readonly ILog Log;

		static AbstractTestFixture()
		{
			Log = LogManager.GetLogger("Test");
		}

		private int OutputTestName(Type type)
		{
			int depth = 0;
			if (type.BaseType != null)
				depth += OutputTestName(type.BaseType);

			if (type.GetAttribute<ScenarioAttribute>() == null)
				return 0;

			Log.Info(new string(' ', depth * 4) + type.Name.Replace("_", " "));

			return depth + 1;
		}
	}
}