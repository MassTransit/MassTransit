// Copyright 2007-2008 The Apache Software Foundation.
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
namespace MassTransit.SystemView.ViewModel
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;

    public class KeyedCollectionBase<T, K> :
        NotifyCollectionChangedBase<T>,
        IList<T>  
        where T : class, IKeyedObject<K>
    {
        protected readonly IDictionary<K, T> Items = new Dictionary<K, T>();

        public virtual T Get(K key)
        {
            return Items[key];
        }

        public virtual IEnumerator<T> GetEnumerator()
        {
            return Items.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public virtual void Add(T item)
        {
            if (Contains(item)) return;
            Items.Add(item.Key, item);
            OnCollectionChanged(NotifyCollectionChangedAction.Add, item);
        }

        public virtual void Clear()
        {
            var removedItems = Items.Values.ToList();
            Items.Clear();
            OnCollectionChanged(NotifyCollectionChangedAction.Reset, removedItems);
        }

        public virtual bool Contains(K key)
        {
            return Items.Keys.Contains(key);
        }

        public virtual bool Contains(T item)
        {
            return Items.Keys.Contains(item.Key);
        }

        public virtual void CopyTo(T[] array, int arrayIndex)
        {
            Items.Values.CopyTo(array, arrayIndex);
        }

        public virtual bool Remove(T item)
        {
            item = Items[item.Key];
            var retValue = Items.Remove(item.Key);
            OnCollectionChanged(NotifyCollectionChangedAction.Remove, item);
            return retValue;
        }

        public virtual int Count
        {
            get { return Items.Count; }
        }

        public virtual bool IsReadOnly
        {
            get { return Items.IsReadOnly; }
        }

        public virtual int IndexOf(T item)
        {
            throw new NotImplementedException();
        }

        public virtual void Insert(int index, T item)
        {
            throw new NotImplementedException();
        }

        public virtual void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public virtual T this[int index]
        {
            get { return Items.Values.ToList()[index]; }
            set { throw new NotImplementedException(); }
        }

        public virtual T this[K key] { get { return Get(key); } }
    }
}