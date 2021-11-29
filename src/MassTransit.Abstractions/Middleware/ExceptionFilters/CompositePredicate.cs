namespace MassTransit.ExceptionFilters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;


    class CompositePredicate<T>
    {
        readonly List<Func<T, bool>> _list = new List<Func<T, bool>>();
        Func<T, bool> _matchesAll = x => true;
        Func<T, bool> _matchesAny = x => true;
        Func<T, bool> _matchesNone = x => false;

        public void Add(Func<T, bool> filter)
        {
            _matchesAll = x => _list.All(predicate => predicate(x));
            _matchesAny = x => _list.Any(predicate => predicate(x));
            _matchesNone = x => !MatchesAny(x);

            _list.Add(filter);
        }

        public static CompositePredicate<T> operator +(CompositePredicate<T> invokes, Func<T, bool> filter)
        {
            invokes.Add(filter);
            return invokes;
        }

        public bool MatchesAll(T target)
        {
            return _matchesAll(target);
        }

        public bool MatchesAny(T target)
        {
            return _matchesAny(target);
        }

        public bool MatchesNone(T target)
        {
            return _matchesNone(target);
        }

        public bool DoesNotMatchAny(T target)
        {
            return _list.Count == 0 || !MatchesAny(target);
        }
    }
}
