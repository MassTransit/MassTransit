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
namespace MassTransit.Transports.Msmq
{
    using System;
    using Serialization;

    public class CreateMsmqEndpointSettings :
        CreateMsmqTransportSettings
    {
        public CreateMsmqEndpointSettings(string endpointUri)
            : base(endpointUri)
        {
            SetDefaultErrorAddress();
        }

        public CreateMsmqEndpointSettings(Uri endpointUri)
            : base(endpointUri)
        {
            SetDefaultErrorAddress();
        }

        public CreateMsmqEndpointSettings(IEndpointAddress endpointAddress)
            : base(endpointAddress)
        {
            SetDefaultErrorAddress();
        }

        public CreateMsmqEndpointSettings()
        {
        }

        private void SetDefaultErrorAddress()
        {
        	Uri uri = Address.Uri;
        	var errorUri = new UriBuilder(uri.Scheme, uri.Host, uri.Port, uri.AbsolutePath + "_error", uri.Query);

        	ErrorAddress = new MsmqEndpointAddress(errorUri.Uri);
        }

        /// <summary>
        /// The address of the endpoint where invalid messages should be moved
        /// </summary>
        public IMsmqEndpointAddress ErrorAddress { get; set; }

        /// <summary>
        /// True if any existing messages at the endpoint should be purged when the endpoint is created
        /// </summary>
        public bool PurgeExistingMessages { get; set; }

        /// <summary>
        /// The serializer to use for messages on the endpoint
        /// </summary>
        public IMessageSerializer Serializer { get; set; }
    }
}