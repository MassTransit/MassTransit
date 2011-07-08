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
namespace MassTransit.Configurators
{
	public static class ValidationResultExtensions
	{
		public static ValidationResult Failure(this Configurator configurator, string message)
		{
			return new ValidationResultImpl(ValidationResultDisposition.Failure, message);
		}

		public static ValidationResult Failure(this Configurator configurator, string key, string message)
		{
			return new ValidationResultImpl(ValidationResultDisposition.Failure, key, message);
		}

		public static ValidationResult Failure(this Configurator configurator, string key, string value, string message)
		{
			return new ValidationResultImpl(ValidationResultDisposition.Failure, key, value, message);
		}

		public static ValidationResult Warning(this Configurator configurator, string message)
		{
			return new ValidationResultImpl(ValidationResultDisposition.Warning, message);
		}

		public static ValidationResult Warning(this Configurator configurator, string key, string message)
		{
			return new ValidationResultImpl(ValidationResultDisposition.Warning, key, message);
		}

		public static ValidationResult Warning(this Configurator configurator, string key, string value, string message)
		{
			return new ValidationResultImpl(ValidationResultDisposition.Warning, key, value, message);
		}

		public static ValidationResult Success(this Configurator configurator, string message)
		{
			return new ValidationResultImpl(ValidationResultDisposition.Success, message);
		}

		public static ValidationResult Success(this Configurator configurator, string key, string message)
		{
			return new ValidationResultImpl(ValidationResultDisposition.Success, key, message);
		}

		public static ValidationResult Success(this Configurator configurator, string key, string value, string message)
		{
			return new ValidationResultImpl(ValidationResultDisposition.Success, key, value, message);
		}

		public static ValidationResult WithParentKey(this ValidationResult result, string parentKey)
		{
			//string key = result.Key.Contains(".") ? result.Key.Substring(result.Key.IndexOf('.')) : "";

			string key = parentKey + "." + result.Key;

			return new ValidationResultImpl(result.Disposition, key, result.Value, result.Message);
		}
	}
}