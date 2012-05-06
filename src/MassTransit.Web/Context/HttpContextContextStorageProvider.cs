// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Context
{
	using System.Web;

	public class HttpContextContextStorageProvider :
		ContextStorageProvider
	{
		const string ReceiveContextKey = "MassTransitReceiveContext";

		readonly ContextStorageProvider _fallback;

		public HttpContextContextStorageProvider()
		{
			_fallback = new ThreadStaticContextStorageProvider();
		}

		public IConsumeContext ConsumeContext
		{
			get
			{
				HttpContext httpContext = HttpContext.Current;
				if (httpContext != null)
				{
					var value = httpContext.Items[ReceiveContextKey] as IConsumeContext;
					return value;
				}

				return _fallback.ConsumeContext;
			}

			set
			{
				HttpContext httpContext = HttpContext.Current;
				if (httpContext != null)
				{
					httpContext.Items[ReceiveContextKey] = value;
				}
				else
				{
					_fallback.ConsumeContext = value;
				}
			}
		}
	}
}