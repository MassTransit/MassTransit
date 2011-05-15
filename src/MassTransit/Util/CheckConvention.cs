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
	using Exceptions;
	using Magnum;
	using Magnum.Extensions;

	/// <summary>
	///   Check class for verifying the condition of items included in interface contracts
	/// </summary>
	public static class CheckConvention
	{
		public static void EnsureSerializable(object message)
		{
			Guard.AgainstNull(message, "message cannot be null");

			Type t = message.GetType();
			if (!t.IsSerializable)
			{
				throw new ConventionException(
					"Whoa, slow down buddy. The message '{0}' must be marked with the 'Serializable' attribute!".FormatWith(t.FullName));
			}
		}
	}
}