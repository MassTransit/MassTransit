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
    using System;
    using System.Threading;


    public class Bucket<TValue>
        where TValue : class
    {
        readonly INodeTracker<TValue> _tracker;
        int _count;
        IBucketNode<TValue> _first;
        DateTime _startTime;
        DateTime? _stopTime;

        public Bucket(INodeTracker<TValue> tracker)
        {
            _tracker = tracker;
        }

        public IBucketNode<TValue> First => _first;

        public int Count => _count;

        public bool HasExpired(TimeSpan maxAge, DateTime now)
        {
            return _startTime < now.Subtract(maxAge);
        }

        public bool IsOldEnough(TimeSpan minAge, DateTime now)
        {
            return now - _stopTime > minAge;
        }

        public void Empty()
        {
            _first = null;
            _count = 0;
        }

        public void Stop(DateTime now)
        {
            _stopTime = now;
        }

        public void Start(DateTime now)
        {
            _startTime = now;
            _stopTime = default(DateTime?);

            _first = null;
            _count = 0;
        }

        /// <summary>
        /// Push a node to the front of the bucket, and set the node's bucket to this bucket
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public IBucketNode<TValue> Push(IBucketNode<TValue> node)
        {
            IBucketNode<TValue> next = _first;

            _first = node;

            node.SetBucket(this, next);

            Interlocked.Increment(ref _count);

            return next;
        }

        /// <summary>
        /// When a node is used, check and rebucket if necessary to keep it in the cache
        /// </summary>
        /// <param name="node"></param>
        public void Used(IBucketNode<TValue> node)
        {
            // a stopped bucket is no longer the current bucket, so give the node back to the manager
            if (_stopTime.HasValue)
            {
                _tracker.Rebucket(node);

                Interlocked.Decrement(ref _count);
            }
        }
    }
}