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
namespace MassTransit.SystemView.Core.ViewModel
{
    using System;

    public class Endpoint :
        IKeyedObject<Uri>
    {
        public Endpoint(Uri endpointUri) : 
            this()
        {
            EndpointUri = endpointUri;
        }

        private Endpoint()
        {
            Messages = new Messages();
            Workers = new Workers();
        }

        public Uri EndpointUri { get; set; }
        public Messages Messages { get; private set; }
        public Workers Workers { get; private set; }
        
        public Uri Key
        {
            get { return EndpointUri; }
        }
    }
}