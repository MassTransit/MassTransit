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
namespace MassTransit.Context
{
	using System;
	using System.IO;
	using Serialization;

	public interface IReceiveContext :
		IConsumeContext
	{
		void SetMessageId(string value);

		void SetInputAddress(Uri uri);

		void SetEndpoint(IEndpoint endpoint);

		void SetBus(IServiceBus bus);

		void SetSourceAddress(Uri uri);

		void SetDestinationAddress(Uri uri);

		void SetResponseAddress(Uri uri);

		void SetFaultAddress(Uri uri);

		void SetNetwork(string value);

		void SetRetryCount(int retryCount);

		void SetExpirationTime(DateTime value);

		void SetMessageType(string messageType);

		void CopyBodyTo(Stream stream);

		Stream BodyStream { get; }

		void SetSerializer(IMessageSerializer serializer);
	}
}