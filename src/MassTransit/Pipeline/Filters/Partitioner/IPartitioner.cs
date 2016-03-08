// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Pipeline.Filters.Partitioner
{
    using System.Threading.Tasks;


    public interface IPartitioner :
        IProbeSite
    {
        IPartitioner<T> GetPartitioner<T>(PartitionKeyProvider<T> keyProvider)
            where T : class, PipeContext;
    }


    public interface IPartitioner<T> :
        IProbeSite
        where T : class, PipeContext
    {
        /// <summary>
        /// Sends the context through the partitioner
        /// </summary>
        /// <typeparam name="T">The context type</typeparam>
        /// <param name="context">The context</param>
        /// <param name="next">The next pipe</param>
        /// <returns></returns>
        Task Send(T context, IPipe<T> next);
    }
}