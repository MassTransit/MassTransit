namespace MassTransit.Infrastructure.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Magnum.Common.Repository;
    using ServiceBus.Timeout;
    using ServiceBus.Util;

    public class PersistantTimeoutStorage :
        ITimeoutRepository

	{
        private readonly IRepository<ScheduledTimeout, Guid> _repository;

        public PersistantTimeoutStorage(IRepository<ScheduledTimeout, Guid> repository)
        {
            _repository = repository;
        }

        public void Schedule(Guid id, DateTime timeoutAt)
        {
            ScheduledTimeout to = new ScheduledTimeout(id, timeoutAt);
            _repository.Save(to);
        }

        public void Remove(Guid id)
        {
            _repository.Delete(_repository.Get(id));
        }

        public IList<Tuple<Guid, DateTime>> List()
        {
            var items = _repository.List();
            IList<Tuple<Guid, DateTime>> result = new List<Tuple<Guid, DateTime>>();
            foreach (var item in items)
            {
                result.Add(new Tuple<Guid, DateTime>(item.Id, item.ExpiresAt));
            }
            return result;
        }

    	public IList<Tuple<Guid, DateTime>> List(DateTime lessThan)
    	{
    		var query = from item in _repository where item.ExpiresAt <= lessThan select item;

			IList<Tuple<Guid, DateTime>> result = new List<Tuple<Guid, DateTime>>();
			foreach (ScheduledTimeout item in query)
			{
				result.Add(new Tuple<Guid, DateTime>(item.Id, item.ExpiresAt));
			}
			return result;
		}

        public event Action<Guid> TimeoutAdded;
        public event Action<Guid> TimeoutUpdated;
        public event Action<Guid> TimeoutRemoved;
    }
}