namespace MassTransit.MongoDbIntegration.Courier
{
    using System;
    using System.Threading.Tasks;
    using Documents;
    using MongoDB.Driver;
    using RetryPolicies;


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
                var result = await _collection.UpdateOneAsync(filterDefinition, update, new UpdateOptions { IsUpsert = true }).ConfigureAwait(false);

                if (!result.IsAcknowledged)
                    throw new MongoDbSaveEventException(trackingNumber, "Write was not acknowledged");

                if (result.UpsertedId == null)
                {
                    if (result.IsModifiedCountAvailable && result.ModifiedCount != 1)
                        throw new MongoDbSaveEventException(trackingNumber, $"Multiple documents were modified: {result.ModifiedCount}");
                }
            });
        }
    }
}
