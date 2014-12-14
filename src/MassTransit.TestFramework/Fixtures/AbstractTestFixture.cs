// Copyright 2007-2010 The Apache Software Foundation.
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
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using Logging;
	using Magnum.Extensions;
	using NUnit.Framework;
	using NUnit.Framework;

	[TestFixture]
	public class AbstractTestFixture
	{
		[TestFixtureSetUp]
		public void AbstractTestFixtureSetup()
		{
			OutputTestName(GetType());

			_timer = Stopwatch.StartNew();	
		}

		[TestFixtureTearDown]
		public void AbstractTestFixtureTeardown()
		{
            if (_timer != null)
            {
                _timer.Stop();
                Log.InfoFormat("Elapsed Time: {0}ms", _timer.ElapsedMilliseconds);
            }
		    
			Log.Info("");
		}

		private Stopwatch _timer;

		protected static readonly ILog Log;

		static AbstractTestFixture()
		{
			Log = Logger.Get("Test");
		}

		private void OutputTestName(Type type)
		{
			string prefix = "";
			int depth = -1;

			GetTestStack(type)
				.Where(x => x.GetAttribute<TestFixtureAttribute>() != null)
				.Select(x => x.Name.Replace("_", " "))
				.Each(x =>
					{
						string s = x.Split(' ')[0];
						if (s != prefix)
						{
							depth++;
							prefix = s;
						}
						else
						{
							x = "And" + x.Substring(prefix.Length);
						}

						Log.Info(new string(' ', depth*4) + x);
					});
		}

		private static IEnumerable<Type> GetTestStack(Type type)
		{
			if (type.BaseType != null)
			{
				foreach (Type next in GetTestStack(type.BaseType))
				{
					yield return next;
				}
			}

			yield return type;
		}
	}
}