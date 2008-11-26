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
namespace MassTransit.Subscriptions.ServerHandlers
{
    using System;
    using log4net;
    using Messages;

    public class AddSubscriptionHandler :
        Consumes<AddSubscription>.All
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (AddSubscriptionHandler));
        private readonly ISubscriptionCache _cache;
        private readonly FollowerRepository _followers;
        private readonly ISubscriptionRepository _repository;


        public AddSubscriptionHandler(ISubscriptionCache cache, ISubscriptionRepository repository, FollowerRepository followers)
        {
            _cache = cache;
            _repository = repository;
            _followers = followers;
        }

        public void Consume(AddSubscription message)
        {
            try
            {
                _cache.Add(message.Subscription);

                _repository.Save(message.Subscription);

                _followers.NotifyFollowers(message);
            }
            catch (Exception ex)
            {
                _log.Error("Exception handling subscription change", ex);
            }
        }
    }
}