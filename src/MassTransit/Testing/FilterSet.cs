namespace MassTransit.Testing
{
    using System.Collections.Generic;
    using System.Linq;


    public class FilterSet<T>
        where T : class
    {
        readonly List<FilterDelegate<T>> _list;
        FilterDelegate<T> _all = x => true;
        FilterDelegate<T> _any = x => true;
        FilterDelegate<T> _notAny = x => false;

        protected FilterSet()
        {
            _list = new List<FilterDelegate<T>>();
        }

        protected FilterSet<T> Add(FilterDelegate<T> filter)
        {
            _all = x => _list.All(predicate => predicate(x));
            _any = x => _list.Any(predicate => predicate(x));
            _notAny = x => !_any(x);

            _list.Add(filter);

            return this;
        }

        public bool All(T target)
        {
            return _all(target);
        }

        public bool Any(T target)
        {
            return _any(target);
        }

        public bool NotAny(T target)
        {
            return _notAny(target);
        }

        public bool None(T target)
        {
            return _list.Count == 0 || !_any(target);
        }
    }
}
