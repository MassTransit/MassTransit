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
namespace MassTransit.Testing
{
    using System;
    using System.Threading;


    /// <summary>
	/// A test scenario that allows the tester to
	/// get hold of what messages were published, skipped, sent and received.
	/// Inherits IDisposable.
	/// </summary>
	public interface ITestScenario :
		IDisposable
	{
		/// <summary>
		/// The list of published messages is contained within this instance.
		/// </summary>
		IPublishedMessageList Published { get; }

		/// <summary>
		/// The list of received messages is contained within this instance.
		/// </summary>
		IReceivedMessageList Received { get; }

		/// <summary>
		/// The list of send messages is contained within this instance.
		/// </summary>
		ISentMessageList Sent { get; }

		/// <summary>
		/// The list of skipped messages is contained within this instance.
		/// </summary>
		IReceivedMessageList Skipped { get; }

        /// <summary>
        /// The cancellation token for the test, use for any awaited items that are part of the text execution.
        /// </summary>
        CancellationToken CancellationToken { get; }

        /// <summary>
        /// The timeout value to be used when a cancellation token is not allowed for the test timeout.
        /// </summary>
        TimeSpan Timeout { get; }

        /// <summary>
        /// Cancel the test execution, if possible
        /// </summary>
        void Cancel();
	}
}