// Copyright 2007-2013 Chris Patterson
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
namespace MassTransit.Steward.Core.Distribution.ConsistentHashing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;


    public class ConsistentHashDistributionStrategy<T> :
        DistributionStrategy<T>
    {
        const int DefaultReplicationCount = 160;
        readonly SortedDictionary<int, T> _circle = new SortedDictionary<int, T>();
        readonly HashGenerator _hashGenerator;
        readonly HashKeyProvider<T> _keyProvider;
        readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        readonly int _replicate;
        Lazy<int[]> _keyCache;

        public ConsistentHashDistributionStrategy(HashGenerator hashGenerator, HashKeyProvider<T> keyProvider,
            int replicate = DefaultReplicationCount)
        {
            _hashGenerator = hashGenerator;
            _keyProvider = keyProvider;
            _replicate = replicate;
            _keyCache = RebuildKeyCache();
        }

        public void Init(IEnumerable<T> nodes)
        {
            _lock.EnterWriteLock();
            try
            {
                foreach (T node in nodes)
                    InternalAdd(node);

                _keyCache = RebuildKeyCache();
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public void Add(T node)
        {
            _lock.EnterWriteLock();
            try
            {
                InternalAdd(node);

                _keyCache = RebuildKeyCache();
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public void Remove(T node)
        {
            _lock.EnterWriteLock();
            try
            {
                for (int i = 0; i < _replicate; i++)
                {
                    var hash = (int)_hashGenerator.Hash(_keyProvider(node) + i);
                    if (!_circle.Remove(hash))
                        throw new Exception("can not remove a node that not added");
                }

                _keyCache = RebuildKeyCache();
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public T this[string key]
        {
            get
            {
                var hash = (int)_hashGenerator.Hash(key);

                return GetNode(hash);
            }
        }

        public T GetNode(byte[] data)
        {
            return GetNode((int)_hashGenerator.Hash(data));
        }

        Lazy<int[]> RebuildKeyCache()
        {
            return new Lazy<int[]>(() => _circle.Keys.ToArray(), LazyThreadSafetyMode.ExecutionAndPublication);
        }

        T GetNode(int hash)
        {
            _lock.EnterUpgradeableReadLock();
            try
            {
                int first = FirstGreaterThanOrEqual(_keyCache.Value, hash);

                return _circle[_keyCache.Value[first]];
            }
            finally
            {
                _lock.ExitUpgradeableReadLock();
            }
        }

        void InternalAdd(T node)
        {
            for (int i = 0; i < _replicate; i++)
            {
                var hash = (int)_hashGenerator.Hash(_keyProvider(node) + i);
                _circle[hash] = node;
            }
        }

        int FirstGreaterThanOrEqual(int[] keyCache, int value)
        {
            int begin = 0;
            int end = keyCache.Length - 1;

            if (keyCache[end] < value || keyCache[0] > value)
                return 0;

            while (end - begin > 1)
            {
                int mid = (end + begin) / 2;
                if (keyCache[mid] >= value)
                    end = mid;
                else
                    begin = mid;
            }

            if (keyCache[begin] > value || keyCache[end] < value)
                throw new Exception("Something went seriously wrong here");

            return end;
        }
    }
}