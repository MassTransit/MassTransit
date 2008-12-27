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
namespace MassTransit
{
	using System;

	/// <summary>
	/// Used by consumers of endpoints to select messages to receive
	/// </summary>
	public interface IMessageSelector :
		IDisposable
	{
		/// <summary>
		/// Called to accept the message contained in this selector. Returns true if the
		/// messages was still available.
		/// </summary>
		/// <returns>True if the message could be accepted, otherwise false.</returns>
		bool AcceptMessage();

		/// <summary>
		/// Moves the message to the specified endpoint
		/// </summary>
		/// <param name="endpoint"></param>
		void MoveMessageTo(IEndpoint endpoint);

		/// <summary>
		/// Deserialize the message and return the message object
		/// </summary>
		/// <returns>The message object</returns>
		object DeserializeMessage();
	}
}