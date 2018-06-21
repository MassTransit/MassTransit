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
    using System.Threading.Tasks;


    /// <summary>
    /// A bucket node has been stored in a bucket, and is a fully resolved value.
    /// </summary>
    /// <typeparam name="TValue">The value type</typeparam>
    public class BucketNode<TValue> :
        IBucketNode<TValue>
        where TValue : class
    {
        Bucket<TValue> _bucket;
        IBucketNode<TValue> _next;
        Task<TValue> _value;

        public BucketNode(TValue value)
        {
            _value = Task.FromResult(value);

            var notify = value as INotifyValueUsed;
            if (notify != null)
                notify.Used += Used;
        }

        public Task<TValue> Value
        {
            get
            {
                Used();
                return _value;
            }
        }

        public bool HasValue => true;
        public Bucket<TValue> Bucket => _bucket;
        public IBucketNode<TValue> Next => _next;

        public Task<TValue> GetValue(IPendingValue<TValue> pendingValue)
        {
            return _value;
        }

        public void SetBucket(Bucket<TValue> bucket, IBucketNode<TValue> next)
        {
            _bucket = bucket;
            _next = next;
        }

        public void SetBucket(Bucket<TValue> bucket)
        {
            _bucket = bucket;
        }

        public void Evict()
        {
            _bucket = null;
            _next = null;

            var notify = _value.Result as INotifyValueUsed;
            if (notify != null)
                notify.Used -= Used;

            Interlocked.Exchange(ref _value, Cached.Removed);
        }

        public IBucketNode<TValue> Pop()
        {
            IBucketNode<TValue> next = _next;

            _next = null;

            return next;
        }

        void Used()
        {
            _bucket?.Used(this);
        }


        static class Cached
        {
            internal static readonly Task<TValue> Removed;

            static Cached()
            {
                var source = new TaskCompletionSource<TValue>();
                source.TrySetException(new InvalidOperationException("The cached value has been removed"));

                Removed = source.Task;
            }
        }
    }
}