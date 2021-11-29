namespace MassTransit.MongoDbIntegration.MessageData
{
    using System;
    using MongoDB.Bson;


    public class MessageDataResolver :
        IMessageDataResolver
    {
        const string Scheme = "urn";
        const string System = "mongodb";
        const string Specification = "gridfs";

        readonly string _format = string.Join(":", Scheme, System, Specification);

        public ObjectId GetObjectId(Uri address)
        {
            if (address.Scheme != Scheme)
                throw new UriFormatException($"The scheme did not match the expected value: {Scheme}");

            string[] tokens = address.AbsolutePath.Split(':');

            if (tokens.Length != 3 || !address.AbsoluteUri.StartsWith($"{_format}:"))
                throw new UriFormatException($"Urn is not in the correct format. Use '{_format}:{{resourceId}}'");

            return ObjectId.Parse(tokens[2]);
        }

        public Uri GetAddress(ObjectId id)
        {
            return new Uri($"{_format}:{id}");
        }
    }
}
