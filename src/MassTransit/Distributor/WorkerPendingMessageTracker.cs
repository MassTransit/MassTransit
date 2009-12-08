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
namespace MassTransit.Distributor
{
    using System.Collections.Generic;
    using Magnum.Threading;

    public class WorkerPendingMessageTracker<T>
    {
        private readonly ReaderWriterLockedObject<HashSet<T>> _messages = new ReaderWriterLockedObject<HashSet<T>>(new HashSet<T>());

        public void Viewed(T item)
        {
            _messages.WriteLock(x =>
            {
                if (!x.Contains(item))
                    x.Add(item);
            });
        }

        public void Consumed(T item)
        {
            _messages.WriteLock(x =>
            {
                if (x.Contains(item))
                    x.Remove(item);
            });
        }

        public int PendingMessagesCount()
        {
            int count = 0;

            _messages.ReadLock(x => count = x.Count);

            return count;
        }
    }
}