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


    public static class UriExtensions
	{
		public static Uri AppendToPath(this Uri uri, string value)
		{
			return new Uri(string.Format("{0}://{1}{2}{3}{4}",uri.Scheme,
				string.Format("{0}{1}", !string.IsNullOrEmpty(uri.UserInfo) ? uri.UserInfo + "@" : "", uri.Host),
				uri.Port < 1 ? "" : ":" + uri.Port,
				uri.AbsolutePath + value,
				uri.Query));

			//return new Uri(uri.ToString().Replace(uri.AbsolutePath, uri.AbsolutePath + value));

		}

        internal static Uri ToUri(this string uriString, string message)
		{
			try
			{
				return new Uri(uriString);
			}
			catch (UriFormatException ex)
			{
				throw new ConfigurationException($"{message}: {uriString}", ex);
			}
		}
	}
}
