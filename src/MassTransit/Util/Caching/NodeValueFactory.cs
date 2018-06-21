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
    using System.Collections.Concurrent;
    using System.Threading.Tasks;


    /// <summary>
    /// A factory for a node which keeps track of subsequent attempts to create the
    /// same node, passing through until a valid node is created.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class NodeValueFactory<TValue> :
        INodeValueFactory<TValue>
        where TValue : class
    {
        readonly BlockingCollection<IPendingValue<TValue>> _pendingCollection;
        readonly int _timeout;
        readonly TaskCompletionSource<TValue> _value;

        /// <summary>
        /// Creates a node value factory, with the inital pending value
        /// </summary>
        /// <param name="initialPendingValue">The value that brought the node to the cache</param>
        /// <param name="timeoutInMilliseconds">The timeout to wait for additional factories before faulting</param>
        public NodeValueFactory(IPendingValue<TValue> initialPendingValue, int timeoutInMilliseconds)
        {
            _timeout = timeoutInMilliseconds;

            _pendingCollection = new BlockingCollection<IPendingValue<TValue>>();

            _value = new TaskCompletionSource<TValue>();

            _pendingCollection.Add(initialPendingValue);
        }

        public Task<TValue> Value => _value.Task;

        public void Add(IPendingValue<TValue> pendingValue)
        {
            if (_value.Task.Status != TaskStatus.WaitingForActivation)
            {
                pendingValue.SetValue(_value.Task);
            }
            else
            {
                _pendingCollection.Add(pendingValue);
            }
        }

        public async Task<TValue> CreateValue()
        {
            Exception lastException = null;

            try
            {
                IPendingValue<TValue> pendingValue;
                while (_pendingCollection.TryTake(out pendingValue, _timeout))
                {
                    try
                    {
                        var value = await pendingValue.CreateValue().ConfigureAwait(false);

                        _value.TrySetResult(value);

                        CompletePendingValues();

                        return value;
                    }
                    catch (Exception ex)
                    {
                        lastException = ex;
                    }
                }
            }
            finally
            {
                _pendingCollection.Dispose();
            }

            var exception = lastException ?? new ValueFactoryException("The value factory method faulted.");

            _value.TrySetException(exception);

            throw exception;
        }

        void CompletePendingValues()
        {
            IPendingValue<TValue> pendingValue;
            while (_pendingCollection.TryTake(out pendingValue, _timeout))
            {
                pendingValue.SetValue(_value.Task);
            }

            _pendingCollection.CompleteAdding();
        }
    }
}