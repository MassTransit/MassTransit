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
namespace MassTransit.Subscriptions
{
    using System;
    using System.Collections.Generic;
    using Internal;
    using Messages;

    public class FollowerRepository
    {
        private readonly IEndpointResolver _endpointResolver;
        private readonly IList<Uri> _followers;

        public FollowerRepository(IEndpointResolver endpointResolver)
        {
            _followers = new List<Uri>();
            _endpointResolver = endpointResolver;
        }

        public void AddFollower(Uri uri)
        {
            lock (_followers)
            {
                if (_followers.Contains(uri))
                    return;

                _followers.Add(uri);
            }
        }

        public void RemoveFollower(Uri uri)
        {
            lock (_followers)
            {
                if (_followers.Contains(uri))
                    _followers.Remove(uri);
            }
        }

        public void NotifyFollowers<T>(T message) where T : SubscriptionChange
        {
            IList<Uri> copy;
            lock (_followers)
                copy = new List<Uri>(_followers);

            foreach (Uri uri in copy)
            {
                // don't send updates to the originator, that's chatty kathy
                if (message.Subscription.EndpointUri == uri)
                    continue;

                IEndpoint ep = _endpointResolver.Resolve(uri);
                ep.Send(message);
            }
        }
    }
}