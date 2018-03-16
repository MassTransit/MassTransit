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
namespace MassTransit.Contracts
{
    using System;


    /// <summary>
    /// Returned when a request has been canceled prior to completion
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    public interface RequestCanceled<out TRequest>
        where TRequest : class
    {
        Guid RequestId { get; }

        DateTime Timestamp { get; }

        TRequest Request { get; }
    }
}