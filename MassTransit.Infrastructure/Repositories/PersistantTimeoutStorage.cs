namespace MassTransit.Infrastructure.Repositories
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using ServiceBus.Timeout;
    using ServiceBus.Util;

    public class PersistantTimeoutStorage :
        ITimeoutStorage
    {
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<Guid> GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        public void Schedule(Guid id, DateTime timeoutAt)
        {
            throw new System.NotImplementedException();
        }

        public void Remove(Guid id)
        {
            throw new System.NotImplementedException();
        }

        public IList<Tuple<Guid, DateTime>> List()
        {
            throw new System.NotImplementedException();
        }

        public void Start()
        {
            throw new System.NotImplementedException();
        }

        public void Stop()
        {
            throw new System.NotImplementedException();
        }

        public event Action<Guid> TimeoutAdded;
        public event Action<Guid> TimeoutUpdated;
        public event Action<Guid> TimeoutRemoved;
    }
}