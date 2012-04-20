// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Distributor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Magnum.Caching;
    using Magnum.Reflection;

    public class DistributorWorkerCache :
        IWorkerCache
    {
        readonly Cache<Type, IList<CachedWorker>> _typeWorkers;
        readonly Cache<Uri, CachedWorker> _workers;

        public DistributorWorkerCache()
        {
            _workers = new ConcurrentCache<Uri, CachedWorker>();
            _typeWorkers = new ConcurrentCache<Type, IList<CachedWorker>>(type => new List<CachedWorker>());
        }

        public IWorkerInfo GetWorker(Uri uri, Func<Uri, IWorkerInfo> getWorker)
        {
            CachedWorker result = _workers.Get(uri, x =>
                {
                    IWorkerInfo worker = getWorker(x);

                    return new CachedWorker(worker);
                });

            return result.Worker;
        }

        public IWorkerInfo<TMessage> GetWorker<TMessage>(Uri uri, Func<Uri, IWorkerInfo> getWorker) where TMessage : class
        {
            CachedWorker result = _workers.Get(uri, x =>
                {
                    IWorkerInfo worker = getWorker(x);

                    var cachedWorker = new CachedWorker(worker);
                    
                    _typeWorkers[typeof(TMessage)].Add(cachedWorker);

                    return cachedWorker;
                });

            return result.MessageWorkers.Get(typeof(TMessage), type => new WorkerInfo<TMessage>(result.Worker))
                   as IWorkerInfo<TMessage>;
        }

        public IEnumerable<IWorkerInfo<TMessage>> GetAvailableWorkers<TMessage>(IConsumeContext<TMessage> context,
            IWorkerSelector<TMessage> selector)
            where TMessage : class
        {
            IEnumerable<IWorkerInfo<TMessage>> candidates =
                _typeWorkers[typeof(TMessage)].Select(x => x.GetWorker<TMessage>());

            return selector.SelectWorker(candidates, context);
        }

        class CachedWorker
        {
            public CachedWorker(IWorkerInfo worker)
            {
                Worker = worker;
                MessageWorkers = new GenericTypeCache<IWorkerInfo>(typeof(IWorkerInfo<>),
                    type => (IWorkerInfo)FastActivator.Create(typeof(WorkerInfo<>),new Type[] {type}));
            }

            public IWorkerInfo Worker { get; private set; }
            public Cache<Type, IWorkerInfo> MessageWorkers { get; private set; }

            public IWorkerInfo<T> GetWorker<T>()
                where T : class
            {
                return MessageWorkers[typeof(T)] as IWorkerInfo<T>;
            }
        }
    }
}