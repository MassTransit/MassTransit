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
namespace MassTransit.SystemView.Core.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Windows.Threading;

    public class NotifyCollectionChangedBase<T> :
        INotifyCollectionChanged
        where T : class
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected virtual void OnCollectionChanged(NotifyCollectionChangedAction action, T item)
        {
            OnCollectionChangedThreadSafe(new NotifyCollectionChangedEventArgs(action, item));
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedAction action, T item, int index)
        {
            OnCollectionChangedThreadSafe(new NotifyCollectionChangedEventArgs(action, item, index));
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedAction action, IList<T> items)
        {
            OnCollectionChangedThreadSafe(new NotifyCollectionChangedEventArgs(action, items));
        }

        protected virtual void OnCollectionChangedThreadSafe(NotifyCollectionChangedEventArgs e)
        {

            NotifyCollectionChangedEventHandler eventHandler = CollectionChanged;

            if (eventHandler == null)
                return;

            Delegate[] delegates = eventHandler.GetInvocationList();

            // Walk thru invocation list
            foreach (NotifyCollectionChangedEventHandler handler in delegates)
            {
                DispatcherObject dispatcherObject = handler.Target as DispatcherObject;

                // If the subscriber is a DispatcherObject and different thread
                if (dispatcherObject != null && dispatcherObject.CheckAccess() == false)
                {
                    // Invoke handler in the target dispatcher's thread
                    dispatcherObject.Dispatcher.Invoke(DispatcherPriority.DataBind, handler, this, e);
                }
                else // Execute handler as is
                    handler(this, e);
            }
        }
    }
}