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