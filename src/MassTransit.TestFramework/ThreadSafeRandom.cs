namespace MassTransit.TestFramework
{
    using System;


    public class ThreadSafeRandom
    {
        static readonly Random GlobalSeedGenerator = new Random();

        [ThreadStatic] static Random _localThreadStaticRandom;

        public ThreadSafeRandom()
        {
            if (_localThreadStaticRandom == null)
            {
                int seed;
                lock (GlobalSeedGenerator)
                    seed = GlobalSeedGenerator.Next();
                _localThreadStaticRandom = new Random(seed);
            }
        }

        public int Next()
        {
            return _localThreadStaticRandom.Next();
        }

        public int Next(int maxValue)
        {
            return _localThreadStaticRandom.Next(maxValue);
        }

        public int Next(int minValue, int maxValue)
        {
            return _localThreadStaticRandom.Next(minValue, maxValue);
        }

        public bool NextBool()
        {
            return _localThreadStaticRandom.Next(0, 2) == 0;
        }

        public void NextBytes(byte[] buffer)
        {
            _localThreadStaticRandom.NextBytes(buffer);
        }

        public double NextDouble()
        {
            return _localThreadStaticRandom.NextDouble();
        }
    }
}
