// Copyright 2007-2008 The Apache Software Foundation.
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//   http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.Services.HealthMonitoring.Server
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
            _cache[information.Uri].LastDetectedAt = DateTime.Now;
            OnNewHealthInformation(information);
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
            OnUpdatedHealthInformation(information);
        }

        public event Action<HealthInformation> NewHealthInformation;
        public event Action<HealthInformation> UpdatedHealthInformation;

        private void OnNewHealthInformation(HealthInformation hi)
        {
            Action<HealthInformation> handler = NewHealthInformation;
            if(handler != null)
            {
                handler(hi);
            }
        }

        private void OnUpdatedHealthInformation(HealthInformation hi)
        {
            Action<HealthInformation> handler = UpdatedHealthInformation;
            if (handler != null)
            {
                handler(hi);
            }
        }
    }
}