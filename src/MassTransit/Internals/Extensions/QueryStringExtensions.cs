// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Internals.Extensions
{
    using System;
    using System.Globalization;
    using System.Linq;


    public static class QueryStringExtensions
    {
        public static bool TryGetValueFromQueryString(this Uri uri, string key, out string value)
        {
            string queryString = uri.Query;
            if (string.IsNullOrEmpty(queryString) || queryString.Length <= 1)
            {
                value = null;
                return false;
            }

            value = queryString.Substring(1)
                .Split('&')
                .Select(x =>
                {
                    string[] values = x.Split('=');
                    if (values.Length == 2)
                        return new
                        {
                            Key = values[0],
                            Value = values[1]
                        };
                    return new
                    {
                        Key = values[0],
                        Value = ""
                    };
                })
                .Where(x => string.Compare(x.Key, key, StringComparison.OrdinalIgnoreCase) == 0)
                .Select(x => x.Value)
                .DefaultIfEmpty(null)
                .SingleOrDefault();

            return value != null;
        }

        public static T GetValueFromQueryString<T>(this Uri uri, string key, T defaultValue)
            where T : struct
        {
            if (String.IsNullOrEmpty(uri.Query))
                return defaultValue;

            try
            {
                string value;
                if (!uri.TryGetValueFromQueryString(key, out value) || string.IsNullOrEmpty(value))
                    return defaultValue;

                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                return defaultValue;
            }
        }
    }
}