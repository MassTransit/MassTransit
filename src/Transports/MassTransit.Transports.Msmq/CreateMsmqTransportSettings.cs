// Copyright 2007-2010 The Apache Software Foundation.
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
    using Magnum;

    public class CreateMsmqTransportSettings
    {
        public CreateMsmqTransportSettings(string uri)
            : this()
        {
            Guard.AgainstEmpty(uri, "The URI cannot be null or empty");

            try
            {
                Address = new MsmqEndpointAddress(new Uri(uri));
            }
            catch (UriFormatException ex)
            {
                throw new ArgumentException("The URI is invalid: " + uri, ex);
            }
        }

        public CreateMsmqTransportSettings(Uri uri)
            : this()
        {
            Guard.AgainstNull(uri, "The URI cannot be null");

            Address = new MsmqEndpointAddress(uri);
        }

        public CreateMsmqTransportSettings(IEndpointAddress address)
            : this()
        {
            Guard.AgainstNull(address, "The address cannot be null");

            Address = new MsmqEndpointAddress(address.Uri);
        }

        public CreateMsmqTransportSettings(IEndpointAddress address, CreateMsmqTransportSettings source)
            : this()
        {
            Guard.AgainstNull(address, "The address cannot be null");
            Guard.AgainstNull(source, "The source settings cannot be null");

            Address = new MsmqEndpointAddress(address.Uri);
            Transactional = source.Transactional;
        }

        public CreateMsmqTransportSettings()
        {
            Transactional = true;
            CreateIfMissing = true;
        	RequireTransactional = false;
        }

        /// <summary>
        /// The address of the endpoint
        /// </summary>
        public IEndpointAddress Address { get; set; }

        /// <summary>
        /// The transport should be created if it was not found
        /// </summary>
        public bool CreateIfMissing { get; set; }

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
    }
}