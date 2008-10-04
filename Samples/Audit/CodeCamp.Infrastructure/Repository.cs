namespace CodeCamp.Infrastructure
{
    using System.Collections.Generic;
    using Core;

    public class Repository<T> : IRepository<T>	where T : IIdentifier
    {
        private readonly Dictionary<object, T> _items = new Dictionary<object, T>();

        public Repository(IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                _items.Add(item.Key, item);
            }
        }

        public T Get(object id)
        {
            if (_items.ContainsKey(id))
                return _items[id];

            return default(T);
        }
		
    }
}