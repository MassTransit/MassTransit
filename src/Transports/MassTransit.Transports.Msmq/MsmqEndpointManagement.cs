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
namespace MassTransit.Transports.Msmq
{
    using System;
    using System.Collections.Generic;
    using System.Messaging;
    using System.Security.Principal;
    using Exceptions;
    using Logging;

    public class MsmqEndpointManagement :
        IEndpointManagement
    {
        static readonly string AdministratorsGroupName;
        static readonly string AnonymousLoginAccountName;
        static readonly string EveryoneAccountName;
        static readonly ILog _log = Logger.Get(typeof (MsmqEndpointManagement));
        readonly IMsmqEndpointAddress _address;

        static MsmqEndpointManagement()
        {
            // WellKnowSidType.WorldSid == "Everyone"; 
            // whoda thunk (http://social.msdn.microsoft.com/forums/en-US/netfxbcl/thread/0737f978-a998-453d-9a6a-c348285d7ea3/)

            AdministratorsGroupName = (new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null))
                .Translate(typeof (NTAccount)).ToString();
            EveryoneAccountName = (new SecurityIdentifier(WellKnownSidType.WorldSid, null))
                .Translate(typeof (NTAccount)).ToString();
            AnonymousLoginAccountName = (new SecurityIdentifier(WellKnownSidType.AnonymousSid, null))
                .Translate(typeof (NTAccount)).ToString();
        }

        MsmqEndpointManagement(IEndpointAddress address)
        {
            _address = address as MsmqEndpointAddress ?? new MsmqEndpointAddress(address.Uri);
        }

        public void Create(bool transactional)
        {
            if (!_address.IsLocal)
                throw new InvalidOperationException("A remote queue cannot be created: " + _address);

            if (CheckForExistingQueue(transactional))
                return;

            using (MessageQueue queue = MessageQueue.Create(_address.LocalName, transactional))
            {
                _log.Debug("A queue was created: " + _address + (transactional ? " (transactional)" : ""));

                queue.SetPermissions(AdministratorsGroupName, MessageQueueAccessRights.FullControl,
                    AccessControlEntryType.Allow);
                queue.SetPermissions(EveryoneAccountName, MessageQueueAccessRights.GenericWrite,
                    AccessControlEntryType.Allow);
                queue.SetPermissions(AnonymousLoginAccountName, MessageQueueAccessRights.GenericWrite,
                    AccessControlEntryType.Allow);
            }

            VerifyQueueSendAndReceive();
        }

        public long Count()
        {
            return Count(long.MaxValue);
        }

        public long Count(long limit)
        {
            long count = 0;

            using (var queue = new MessageQueue(_address.InboundFormatName, QueueAccessMode.Receive))
            {
                using (MessageEnumerator enumerator = queue.GetMessageEnumerator2())
                {
                    while (enumerator.MoveNext(TimeSpan.Zero))
                    {
                        if (++count >= limit)
                            break;
                    }
                }
            }

            return count;
        }

        public void Purge()
        {
            using (var queue = new MessageQueue(_address.LocalName, QueueAccessMode.ReceiveAndAdmin))
            {
                queue.Purge();
            }
        }

        public bool Exists
        {
            get { return MessageQueue.Exists(_address.LocalName); }
        }

        public bool IsTransactional
        {
            get
            {
                if (!_address.IsLocal)
                    return true;

                using (var queue = new MessageQueue(_address.InboundFormatName, QueueAccessMode.ReceiveAndAdmin))
                {
                    return queue.Transactional;
                }
            }
        }

        public Dictionary<string, int> MessageTypes()
        {
            var dict = new Dictionary<string, int>();

            using (var queue = new MessageQueue(_address.InboundFormatName, QueueAccessMode.Receive))
            {
                using (MessageEnumerator enumerator = queue.GetMessageEnumerator2())
                {
                    while (enumerator.MoveNext(TimeSpan.Zero))
                    {
                        Message q = enumerator.Current;
                        if (!dict.ContainsKey(q.Label))
                            dict.Add(q.Label, 0);

                        dict[q.Label]++;
                    }
                }
            }

            return dict;
        }

        void VerifyQueueSendAndReceive()
        {
            using (var queue = new MessageQueue(_address.InboundFormatName, QueueAccessMode.SendAndReceive))
            {
                if (!queue.CanRead)
                    throw new InvalidOperationException("A queue was created but cannot be read: " + _address);

                if (!queue.CanWrite)
                    throw new InvalidOperationException("A queue was created but cannot be written: " + _address);
            }
        }

        bool CheckForExistingQueue(bool transactional)
        {
            if (!MessageQueue.Exists(_address.LocalName))
                return false;

            using (var queue = new MessageQueue(_address.InboundFormatName, QueueAccessMode.ReceiveAndAdmin))
            {
                if (transactional && !queue.Transactional)
                {
                    throw new InvalidOperationException(
                        "A non-transactional queue already exists with the same name: " + _address);
                }

                if (!transactional && queue.Transactional)
                {
                    throw new InvalidOperationException("A transactional queue already exists with the same name: " +
                                                        _address);
                }
            }

            _log.Debug("The queue already exists: " + _address);
            return true;
        }

        public static void Manage(IEndpointAddress address, Action<IEndpointManagement> action)
        {
            try
            {
                var management = new MsmqEndpointManagement(address);
                action(management);
            }
            catch (EndpointException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new TransportException(address.Uri, "There was a problem managing the transport", ex);
            }
        }

        public static IEndpointManagement New(Uri uri)
        {
            try
            {
                var address = new MsmqEndpointAddress(uri);

                return New(address);
            }
            catch (UriFormatException ex)
            {
                throw new EndpointException(uri, "The MSMQ queue address is invalid.", ex);
            }
        }

        public static IEndpointManagement New(IEndpointAddress address)
        {
            return new MsmqEndpointManagement(address);
        }
    }
}