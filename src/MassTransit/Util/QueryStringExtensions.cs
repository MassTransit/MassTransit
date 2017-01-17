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
namespace MassTransit.Util
{
	using System;
	using System.Linq;

	public static class QueryStringExtensions
	{
		public static string GetValueFromQueryString(this string queryString, string key)
		{
			if (String.IsNullOrEmpty(queryString) || queryString.Length <= 1)
				return null;

			return queryString.Substring(1)
				.Split('&')
				.Select(x =>
					{
						string[] values = x.Split('=');

						return new {Key = values[0], Value = values[1]};
					})
				.Where(x => string.Compare(x.Key, key, StringComparison.OrdinalIgnoreCase) == 0)
				.Select(x => x.Value)
				.DefaultIfEmpty(null)
				.SingleOrDefault();
		}

		public static T GetValueFromQueryString<T>(this string queryString, string key, T defaultValue)
			where T : struct
		{
			if (String.IsNullOrEmpty(queryString))
				return defaultValue;

			try
			{
				string value = GetValueFromQueryString(queryString, key);
				if (String.IsNullOrEmpty(value))
					return defaultValue;

				return (T) Convert.ChangeType(value, typeof (T));
			}
			catch
			{
				return defaultValue;
			}
		}

		public static T GetValueFromQueryString<T>(this string queryString, string key)
			where T : struct
		{
			return queryString.GetValueFromQueryString(key, default(T));
		}
	}
}