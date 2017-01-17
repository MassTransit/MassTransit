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
namespace MassTransit.MongoDbIntegration.Tests.Courier
{
    using System;
    using MongoDB.Bson;
    using MongoDB.Driver;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public abstract class MongoDbTestFixture :
        InMemoryActivityTestFixture
    {
        protected const string EventCollectionName = "Events";
        protected const string EventDatabaseName = "EventStore";

        protected MongoClient Client;
        protected IMongoDatabase Database { get; set; }

        MassTransitMongoDbConventions _convention;

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            base.ConfigureInMemoryBus(configurator);

            var builder = new MongoUrlBuilder
            {
                DatabaseName = EventDatabaseName,
                Server = new MongoServerAddress("localhost"),
                Username = "test",
                Password = "password",
                ConnectTimeout = TimeSpan.FromSeconds(30),
                ConnectionMode = ConnectionMode.Automatic,
                GuidRepresentation = GuidRepresentation.Standard
            };

            var url = builder.ToMongoUrl();

            Client = new MongoClient("mongodb://127.0.0.1");

            Database = Client.GetDatabase(EventDatabaseName);

            _convention = new MassTransitMongoDbConventions();
        }
    }
}