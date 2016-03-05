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
namespace MassTransit.Context
{
    using System;
    using System.Threading;
    using Util;


    public class PayloadCache :
        IPayloadCache
    {
        IPayloadCollection _collection;

        public PayloadCache()
        {
            _collection = new EmptyPayloadCollection();
        }

        PayloadCache(IReadOnlyPayloadCollection collection)
        {
            _collection = new EmptyPayloadCollection(collection);
        }

        bool IReadOnlyPayloadCollection.HasPayloadType(Type propertyType)
        {
            return _collection.HasPayloadType(propertyType);
        }

        bool IReadOnlyPayloadCollection.TryGetPayload<T>(out T value)
        {
            return _collection.TryGetPayload(out value);
        }

        T IPayloadCache.GetOrAddPayload<T>(PayloadFactory<T> payloadFactory)
        {
            try
            {
                IPayloadValue<T> payload = null;

                IPayloadCollection currentCollection;
                do
                {
                    T existingValue;
                    if (_collection.TryGetPayload(out existingValue))
                        return existingValue;

                    IPayloadValue<T> contextProperty = payload ?? (payload = new PayloadValue<T>(payloadFactory()));

                    currentCollection = Volatile.Read(ref _collection);

                    Interlocked.CompareExchange(ref _collection, currentCollection.Add(contextProperty), currentCollection);
                }
                while (currentCollection == Volatile.Read(ref _collection));

                return payload.Value;
            }
            catch (Exception exception)
            {
                throw new PayloadFactoryException("The payload factory faulted: " + TypeMetadataCache<T>.ShortName, exception);
            }
        }

        IPayloadCache IPayloadCache.CreateScope()
        {
            return new PayloadCache(_collection);
        }
    }
}