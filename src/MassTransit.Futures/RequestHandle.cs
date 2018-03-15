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
    using System.Threading.Tasks;


    public interface RequestHandle<TRequest> :
        MessageHandle<TRequest>,
        RequestHandle
        where TRequest : class
    {
    }


    public interface RequestHandle :
        MessageHandle
    {
        /// <summary>
        /// If the specified result type is present, it is returned.
        /// </summary>
        /// <typeparam name="T">The result type</typeparam>
        /// <returns>True if the result type specified is present, otherwise false</returns>
        Task<Result<T>> GetResult<T>()
            where T : class;
    }
}