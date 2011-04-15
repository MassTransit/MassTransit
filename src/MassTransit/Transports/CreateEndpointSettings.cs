// Copyright 2007-2011 The Apache Software Foundation.
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
    using Magnum;
    using Serialization;

    public class CreateEndpointSettings
    {
        public CreateEndpointSettings(string uri)
        {
            Guard.AgainstNull(uri, "The URI cannot be null or empty");
            Guard.AgainstEmpty(uri, "The URI cannot be null or empty");

            try
            {
                var address = new EndpointAddress(new Uri(uri));
                SetAddresses(address);
            }
            catch (UriFormatException ex)
            {
                throw new ArgumentException("The URI is invalid: " + uri, ex);
            }
        }

        public CreateEndpointSettings(Uri uri)
        {
            Guard.AgainstNull(uri, "The URI cannot be null");

            var address = new EndpointAddress(uri);
            SetAddresses(address);
        }

        public CreateEndpointSettings(IEndpointAddress address)
        {
            Guard.AgainstNull(address, "The address cannot be null");

            SetAddresses(address);
        }

        public CreateEndpointSettings(IEndpointAddress address, CreateEndpointSettings source)
        {
            Guard.AgainstNull(address, "The address cannot be null");
            Guard.AgainstNull(source, "The source settings cannot be null");

            Serializer = source.Serializer;
            Transactional = source.Transactional;
            PurgeExistingMessages = source.PurgeExistingMessages;
            CreateIfMissing = source.CreateIfMissing;


            SetAddresses(address);
        }


        protected void SetAddresses(IEndpointAddress address)
        {
            if(address.IsTransactional)
                Transactional = true;

            Address = address;

            var errorPath = Address.Uri.AbsolutePath + "_error";
            var errorUri = new UriBuilder(Address.Uri.Scheme, Address.Uri.Host, Address.Uri.Port, errorPath).Uri;
            ErrorAddress = new EndpointAddress(errorUri);
        }

        /// <summary>
        /// The address of the endpoint
        /// </summary>
        public IEndpointAddress Address { get; set; }


        /// <summary>
        /// The address of the endpoint where invalid messages should be moved
        /// </summary>
        public IEndpointAddress ErrorAddress { get; set; }

        /// <summary>
        /// The serializer to use for messages on the endpoint
        /// </summary>
        public IMessageSerializer Serializer { get; set; }

        public bool Transactional { get; set; }

        /// <summary>
        /// True if any existing messages at the endpoint should be purged when the endpoint is created
        /// </summary>
        public bool PurgeExistingMessages { get; set; }

        public bool CreateIfMissing { get; set; }

        public CreateTransportSettings ToTransportSettings()
        {
            return new CreateTransportSettings(Address)
                {
                    CreateIfMissing = CreateIfMissing,
                    RequireTransactional = Transactional,
                    Transactional =  Transactional,
                    PurgeExistingMessages = PurgeExistingMessages
                };
        }
    }
}