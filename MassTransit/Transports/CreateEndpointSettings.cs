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
namespace MassTransit.Transports
{
    using System;
    using Magnum;
    using Serialization;

    public class CreateEndpointSettings
    {
        public CreateEndpointSettings(string uri)
            : this()
        {
            Guard.Against.NullOrEmpty(uri, "The URI cannot be null or empty");

            try
            {
                Address = new EndpointAddress(new Uri(uri));
            }
            catch (UriFormatException ex)
            {
                throw new ArgumentException("The URI is invalid: " + uri, ex);
            }
        }

        public CreateEndpointSettings(Uri uri)
            : this()
        {
            Guard.Against.Null(uri, "The URI cannot be null");

            Address = new EndpointAddress(uri);

            SetDefaultErrorAddress();
        }

        public CreateEndpointSettings(IEndpointAddress address)
            : this()
        {
            Guard.Against.Null(address, "The address cannot be null");

            Address = new EndpointAddress(address.Uri);

            SetDefaultErrorAddress();
        }

        public CreateEndpointSettings(IEndpointAddress address, CreateEndpointSettings source)
            : this()
        {
            Guard.Against.Null(address, "The address cannot be null");
            Guard.Against.Null(source, "The source settings cannot be null");

            Address = new EndpointAddress(address.Uri);
            SetDefaultErrorAddress();

            Serializer = source.Serializer;
        }

        public CreateEndpointSettings()
        {
        }

        private void SetDefaultErrorAddress()
        {
            ErrorAddress = new EndpointAddress(new Uri(Address.Uri + "_error"));
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
    }
}