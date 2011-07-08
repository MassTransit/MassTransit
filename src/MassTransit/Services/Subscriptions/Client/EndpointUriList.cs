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
namespace MassTransit.Services.Subscriptions.Client
{
	using System;
	using System.Collections;
	using System.Collections.Generic;

	public class EndpointUriList : 
		IEnumerable<Uri>
	{
		private readonly List<Uri> _endpoints = new List<Uri>();

		public void Add(IEndpoint endpoint)
		{
			_endpoints.Add(endpoint.Address.Uri);
		}

		public void Add(Uri uri)
		{
			_endpoints.Add(uri);
		}

		public bool Contains(IEndpoint endpoint)
		{
			return _endpoints.Contains(endpoint.Address.Uri);
		}

		public bool Contains(Uri uri)
		{
			return _endpoints.Contains(uri);
		}

		public void Remove(Uri uri)
		{
			_endpoints.Remove(uri);
		}

		public IEnumerator<Uri> GetEnumerator()
		{
			return _endpoints.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}