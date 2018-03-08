// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    /// A batch of messages which are delivered to a consumer all at once
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface Batch<out T>
        where T : class
    {
        BatchCompletionMode Mode { get; }

        /// <summary>
        /// When the first message in this batch was received
        /// </summary>
        DateTime FirstMessageReceived { get; }

        /// <summary>
        /// When the last message in this batch was received
        /// </summary>
        DateTime LastMessageReceived { get; }

        /// <summary>
        /// Returns the message at the specified index
        /// </summary>
        /// <param name="index"></param>
        ConsumeContext<T> this[int index] { get; }

        /// <summary>
        /// The number of messages in this batch
        /// </summary>
        int Length { get; }
    }
}