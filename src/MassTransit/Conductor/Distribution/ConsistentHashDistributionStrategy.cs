namespace MassTransit.Conductor.Distribution
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;


    public class ConsistentHashDistributionStrategy<T> :
        IDistributionStrategy<T>
        where T : class
    {
        const byte DefaultReplicationCount = 160;
        readonly SortedDictionary<int, T> _circle = new SortedDictionary<int, T>();
        readonly IHashGenerator _hashGenerator;
        readonly HashKeyProvider<T> _keyProvider;
        readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        readonly byte _replicate;
        Lazy<int[]> _keyCache;

        public ConsistentHashDistributionStrategy(IHashGenerator hashGenerator, HashKeyProvider<T> keyProvider, byte replicate = DefaultReplicationCount)
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
                byte[] key = _keyProvider(node);
                int offset = key.Length;

                Array.Resize(ref key, offset + 1);

                for (byte i = 0; i < _replicate; i++)
                {
                    key[offset] = i;
                    var hash = (int)_hashGenerator.Hash(key);
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
                if (_circle.Keys.Count == 0)
                    return default;

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
            byte[] key = _keyProvider(node);
            int offset = key.Length;

            Array.Resize(ref key, offset + 1);

            for (byte i = 0; i < _replicate; i++)
            {
                key[offset] = i;

                var hash = (int)_hashGenerator.Hash(key);
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
