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
namespace MassTransit.MongoDbIntegration.Courier
{
    using System;
    using System.Threading.Tasks;
    using Documents;
    using GreenPipes;
    using MassTransit.Courier.MongoDbIntegration;
    using MongoDB.Driver;
    using Policies;


    public class RoutingSlipEventPersister :
        IRoutingSlipEventPersister
    {
        readonly IMongoCollection<RoutingSlipDocument> _collection;
        readonly IRetryPolicy _retryPolicy;

        public RoutingSlipEventPersister(IMongoCollection<RoutingSlipDocument> collection)
        {
            _collection = collection;

            _retryPolicy = Retry.Selected<MongoWriteException>().Interval(10, TimeSpan.FromMilliseconds(20));
        }

        Task IRoutingSlipEventPersister.Persist<T>(Guid trackingNumber, T @event)
        {
            FilterDefinition<RoutingSlipDocument> filterDefinition = Builders<RoutingSlipDocument>.Filter.Eq(x => x.TrackingNumber, trackingNumber);

            UpdateDefinition<RoutingSlipDocument> update = Builders<RoutingSlipDocument>.Update.AddToSet(x => x.Events, @event);

            return _retryPolicy.Retry(async () =>
            {
                var result = await _collection.UpdateOneAsync(filterDefinition, update, new UpdateOptions {IsUpsert = true}).ConfigureAwait(false);

                if (!result.IsAcknowledged)
                    throw new SaveEventException(trackingNumber, "Write was not acknowledged");

                if (result.UpsertedId == null)
                {
                    if (result.IsModifiedCountAvailable && result.ModifiedCount != 1)
                        throw new SaveEventException(trackingNumber, $"Multiple documents were modified: {result.ModifiedCount}");
                }
            });
        }
    }
}