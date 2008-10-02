namespace MassTransit.Infrastructure.Repositories
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Magnum.Infrastructure.Repository;
    using ServiceBus.Timeout;
    using ServiceBus.Util;

    public class PersistantTimeoutStorage :
        ITimeoutStorage
    {
        private NHibernateRepository _repository;

        #region ITimeoutStorage Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<Guid> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public void Schedule(Guid id, DateTime timeoutAt)
        {
            ScheduledTimeout to = new ScheduledTimeout(id, timeoutAt);
            _repository.Save(to);
        }

        public void Remove(Guid id)
        {
            _repository.Delete(_repository.Get<ScheduledTimeout>(id));
        }

        public IList<Tuple<Guid, DateTime>> List()
        {
            var items = _repository.List<ScheduledTimeout>();
            IList<Tuple<Guid, DateTime>> result = new List<Tuple<Guid, DateTime>>();
            foreach (var item in items)
            {
                result.Add(new Tuple<Guid, DateTime>(item.Id, item.ExpiresAt));
            }
            return result;
        }

        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public event Action<Guid> TimeoutAdded;
        public event Action<Guid> TimeoutUpdated;
        public event Action<Guid> TimeoutRemoved;

        #endregion
    }
}