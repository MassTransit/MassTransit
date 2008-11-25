namespace MassTransit.Internal
{
    using System.Collections;
    using System.Collections.Generic;

    public class IdempotentList<T> :
        IEnumerable<T>
    {
        private readonly IList<T> _list;

        public IdempotentList()
        {
            _list = new List<T>();
        }

        public void Add(T item)
        {
            if(!_list.Contains(item))
            {
                _list.Add(item);    
            }
        }

        public void Remove(T item)
        {
            if(_list.Contains(item))
            {
                _list.Remove(item);
            }
        }

        public int Count
        {
            get { return _list.Count; }
        }

        #region Implementation of IEnumerable

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Implementation of IEnumerable<T>

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        #endregion
    }
}