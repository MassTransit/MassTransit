namespace MassTransit.MongoDbIntegration.MessageData
{
    using System;
    using MongoDB.Bson;


    public interface IMessageDataResolver
    {
        /// <summary>
        /// Returns the ObjectId for the specified address
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        ObjectId GetObjectId(Uri address);

        /// <summary>
        /// Returns the address for the specified objectId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Uri GetAddress(ObjectId id);
    }
}
