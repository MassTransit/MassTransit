// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Testing
{
	using Configurators;

	public static class TestConfiguratorResultExtensions
	{
		public static TestConfiguratorResult Failure(this ITestConfigurator configurator, string message)
		{
			return new TestConfiguratorResultImpl(TestConfiguratorResultDisposition.Failure, message);
		}

		public static TestConfiguratorResult Failure(this ITestConfigurator configurator, string key, string message)
		{
			return new TestConfiguratorResultImpl(TestConfiguratorResultDisposition.Failure, key, message);
		}

		public static TestConfiguratorResult Failure(this ITestConfigurator configurator, string key, string value,
		                                             string message)
		{
			return new TestConfiguratorResultImpl(TestConfiguratorResultDisposition.Failure, key, value, message);
		}

		public static TestConfiguratorResult Warning(this ITestConfigurator configurator, string message)
		{
			return new TestConfiguratorResultImpl(TestConfiguratorResultDisposition.Warning, message);
		}

		public static TestConfiguratorResult Warning(this ITestConfigurator configurator, string key, string message)
		{
			return new TestConfiguratorResultImpl(TestConfiguratorResultDisposition.Warning, key, message);
		}

		public static TestConfiguratorResult Warning(this ITestConfigurator configurator, string key, string value,
		                                             string message)
		{
			return new TestConfiguratorResultImpl(TestConfiguratorResultDisposition.Warning, key, value, message);
		}

		public static TestConfiguratorResult Success(this ITestConfigurator configurator, string message)
		{
			return new TestConfiguratorResultImpl(TestConfiguratorResultDisposition.Success, message);
		}

		public static TestConfiguratorResult Success(this ITestConfigurator configurator, string key, string message)
		{
			return new TestConfiguratorResultImpl(TestConfiguratorResultDisposition.Success, key, message);
		}

		public static TestConfiguratorResult Success(this ITestConfigurator configurator, string key, string value,
		                                             string message)
		{
			return new TestConfiguratorResultImpl(TestConfiguratorResultDisposition.Success, key, value, message);
		}

		public static TestConfiguratorResult WithParentKey(this TestConfiguratorResult result, string parentKey)
		{
			//string key = result.Key.Contains(".") ? result.Key.Substring(result.Key.IndexOf('.')) : "";

			string key = parentKey + "." + result.Key;

			return new TestConfiguratorResultImpl(result.Disposition, key, result.Value, result.Message);
		}
	}
}