// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Collections.Generic;
    using System.Diagnostics;

    public class PublishContext<T> :
        OldSendContext<T>,
        IBusPublishContext<T>
        where T : class
    {
        readonly IPublishContext _notifySend;
        readonly HashSet<Uri> _endpoints = new HashSet<Uri>();
        Action<IEndpointAddress> _eachSubscriberAction = Ignore;
        Action _noSubscribersAction = Ignore;
        Stopwatch _timer;
        Func<Uri, bool> _wasEndpointAlreadySent;

        PublishContext(T message)
            : base(message)
        {
            _wasEndpointAlreadySent = DefaultEndpointSent;
            _timer = Stopwatch.StartNew();
            _notifySend = null;
        }

        PublishContext(T message, ISendContext context)
            : base(message, context)
        {
            _notifySend = context as IPublishContext;
    
            _wasEndpointAlreadySent = DefaultEndpointSent;
            _timer = Stopwatch.StartNew();
        }

        public TimeSpan Duration
        {
            get { return _timer.Elapsed; }
        }

        public override bool TryGetContext<TMessage>(out IBusPublishContext<TMessage> context)
        {
            context = null;

            if (typeof (TMessage).IsAssignableFrom(typeof (T)))
            {
                var busPublishContext = new PublishContext<TMessage>(Message as TMessage, this);
                busPublishContext._wasEndpointAlreadySent = _wasEndpointAlreadySent;
                busPublishContext.IfNoSubscribers(_noSubscribersAction);
                busPublishContext.ForEachSubscriber(_eachSubscriberAction);

                context = busPublishContext;
            }

            return context != null;
        }

        public override void NotifySend(IEndpointAddress address)
        {
            if (_notifySend != null)
            {
                _notifySend.NotifySend(address);
                return;
            }

            _endpoints.Add(address.Uri);

            base.NotifySend(address);

            _eachSubscriberAction(address);
        }

        public bool WasEndpointAlreadySent(IEndpointAddress address)
        {
            return _wasEndpointAlreadySent(address.Uri);
        }

        public void NotifyNoSubscribers()
        {
            _noSubscribersAction();
        }

        public void IfNoSubscribers(Action action)
        {
            _noSubscribersAction = action;
        }

        public void ForEachSubscriber(Action<IEndpointAddress> action)
        {
            _eachSubscriberAction = action;
        }

        public void Complete()
        {
            _timer.Stop();
        }

        bool DefaultEndpointSent(Uri uri)
        {
            return _endpoints.Contains(uri);
        }

        public static PublishContext<T> FromMessage(T message)
        {
            return new PublishContext<T>(message);
        }

        public static PublishContext<T> FromMessage<TMessage>(TMessage message, ISendContext context)
            where TMessage : class
        {
            if (typeof (TMessage).IsAssignableFrom(typeof (T)))
            {
                return new PublishContext<T>(message as T, context);
            }

            return null;
        }

        static void Ignore()
        {
        }

        static void Ignore(IEndpointAddress endpoint)
        {
        }

        public void SentTo(Uri uri)
        {
            _endpoints.Add(uri);
        }
    }
}