// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Util.Caching
{
    public interface IBucketNode<TValue> :
        INode<TValue>
        where TValue : class
    {
        /// <summary>
        /// The node's bucket
        /// </summary>
        Bucket<TValue> Bucket { get; }

        /// <summary>
        /// Returns the next node in the bucket
        /// </summary>
        IBucketNode<TValue> Next { get; }

        /// <summary>
        /// Puts the node's bucket, once the value is resolved, so that the node
        /// can be tracked.
        /// </summary>
        /// <param name="bucket"></param>
        /// <param name="next"></param>
        void SetBucket(Bucket<TValue> bucket, IBucketNode<TValue> next);

        /// <summary>
        /// Assigns the node to a new bucket, but doesn't change the next node
        /// until it's cleaned up
        /// </summary>
        /// <param name="bucket"></param>
        void SetBucket(Bucket<TValue> bucket);

        /// <summary>
        /// Forcibly evicts the node by setting the internal state to
        /// nothing.
        /// </summary>
        void Evict();

        /// <summary>
        /// Remove the node from the bucket, and return the next node
        /// </summary>
        /// <returns></returns>
        IBucketNode<TValue> Pop();
    }
}