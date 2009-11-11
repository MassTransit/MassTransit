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
namespace MassTransit.Grid
{
	using System;
	using System.Security.Cryptography;
	using System.Text;

	public static class ServiceTypeExtensions
	{
		private static readonly MD5CryptoServiceProvider _cryptoProvider = new MD5CryptoServiceProvider();

		public static Guid ToServiceTypeId(this Type type)
		{
			string key = type.AssemblyQualifiedName;

			var bytes = Encoding.UTF8.GetBytes(key);

			var hash = _cryptoProvider.ComputeHash(bytes);

			return new Guid(hash);
		}
	}
}