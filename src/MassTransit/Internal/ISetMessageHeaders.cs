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
namespace MassTransit.Internal
{
	using System;

	/// <summary>
	/// Used to set the message headers
	/// </summary>
	public interface ISetMessageHeaders :
		IMessageHeaders
	{
		void SetSourceAddress(Uri uri);
		void SetSourceAddress(string uriString);

		void SetDestinationAddress(Uri uri);
		void SetDestinationAddress(string uriString);

		void SetResponseAddress(Uri uri);
		void SetResponseAddress(string uriString);

		void SetFaultAddress(Uri uri);
		void SetFaultAddress(string uriString);

		void SetNetwork(string network);

		void SetRetryCount(int retryCount);

		void SetExpirationTime(DateTime value);

		void SetMessageType(Type messageType);
		void SetMessageType(string messageType);

		void Reset();
	}
}