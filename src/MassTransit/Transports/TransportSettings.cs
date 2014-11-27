// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Transports
{
    using System;
    using System.Transactions;
    using Magnum;
    using Magnum.Extensions;


    public class TransportSettings :
        ITransportSettings
    {
        public TransportSettings(EndpointAddress address)
        {
            Guard.AgainstNull(address, "address");

            Address = address;

            RequireTransactional = false;
            TransactionTimeout = 30.Seconds();
            IsolationLevel = IsolationLevel.Serializable;

            CreateIfMissing = true;
            PurgeExistingMessages = false;
        }

        public TransportSettings(EndpointAddress address, ITransportSettings source)
        {
            Guard.AgainstNull(address, "address");
            Guard.AgainstNull(source, "source");

            Address = address;

            Transactional = source.Transactional;
            RequireTransactional = source.RequireTransactional;
            TransactionTimeout = source.TransactionTimeout;
            IsolationLevel = source.IsolationLevel;

            CreateIfMissing = source.CreateIfMissing;
            PurgeExistingMessages = source.PurgeExistingMessages;
        }

        /// <summary>
        /// The address of the endpoint
        /// </summary>
        public EndpointAddress Address { get; private set; }

        /// <summary>
        /// True if the endpoint should be transactional. If Transactional is true and the endpoint already
        /// exists and is not transactional, an exception will be thrown.
        /// </summary>
        public bool Transactional { get; set; }

        /// <summary>
        /// if the transactional queue is requested and required it will throw an exception if the queue 
        /// exists and is not transactional
        /// </summary>
        public bool RequireTransactional { get; set; }

        /// <summary>
        /// The timeout for the transaction if System.Transactions is supported
        /// </summary>
        public TimeSpan TransactionTimeout { get; set; }

        /// <summary>
        /// The isolation level to use with the transaction if a transactional transport is used
        /// </summary>
        public IsolationLevel IsolationLevel { get; set; }

        /// <summary>
        /// The transport should be created if it was not found
        /// </summary>
        public bool CreateIfMissing { get; set; }

        /// <summary>
        /// If the transport should purge any existing messages before reading from the queue
        /// </summary>
        public bool PurgeExistingMessages { get; set; }
    }
}