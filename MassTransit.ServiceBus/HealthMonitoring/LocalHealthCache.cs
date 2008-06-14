namespace MassTransit.ServiceBus.HealthMonitoring
{
    using System;
    using System.Collections.Generic;
    using Internal;

    public class LocalHealthCache :
        IHealthCache
    {
        private readonly IdempotentHashtable<Uri, HealthInformation> _cache;

        public LocalHealthCache()
        {
            _cache = new IdempotentHashtable<Uri, HealthInformation>();
        }


        public void Add(HealthInformation information)
        {
            _cache.Add(information.Uri, information);
        }

        public IList<HealthInformation> List()
        {
            IList<HealthInformation> result = new List<HealthInformation>();

            foreach (KeyValuePair<Uri, HealthInformation> pair in _cache)
            {
                result.Add(pair.Value);
            }

            return result;
        }

        public HealthInformation Get(Uri uri)
        {
            return _cache[uri];
        }

        public void Update(HealthInformation information)
        {
            _cache[information.Uri] = information;
        }
    }
}