// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Util.Scanning
{
    using System;
    using System.Collections.Generic;
    using System.Linq;


    public class CompositePredicate<T>
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

        public bool DoesNotMatcheAny(T target)
        {
            return _list.Count == 0 || !MatchesAny(target);
        }
    }
}