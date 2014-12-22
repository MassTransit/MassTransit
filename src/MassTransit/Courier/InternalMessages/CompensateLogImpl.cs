// Copyright 2007-2014 Chris Patterson
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
namespace MassTransit.Courier.InternalMessages
{
    using System;
    using System.Collections.Generic;
    using Contracts;


    class CompensateLogImpl :
        CompensateLog
    {
        public CompensateLogImpl(Guid activityTrackingNumber, Uri address,
            IDictionary<string, object> data)
        {
            ActivityTrackingNumber = activityTrackingNumber;
            Address = address;
            Data = data;
        }

        public Guid ActivityTrackingNumber { get; private set; }
        public Uri Address { get; private set; }
        public IDictionary<string, object> Data { get; private set; }
    }
}