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
namespace MassTransit
{
    using System;
    using System.Linq;
    using System.Runtime.Serialization;


    [Serializable]
    public class RequestFaultException :
        RequestException
    {
        public RequestFaultException(string requestType, Fault fault)
            : base($"The {requestType} request faulted: {string.Join(Environment.NewLine, fault.Exceptions.Select(x => x.Message))}")
        {
            RequestType = requestType;
            Fault = fault;
        }

        public RequestFaultException()
        {
        }

        protected RequestFaultException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            RequestType = info.GetString("RequestType");
            Fault = (Fault)info.GetValue("Fault", typeof(Fault));
        }

        public string RequestType { get; private set; }
        public Fault Fault { get; private set; }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("RequestType", RequestType);
            info.AddValue("Fault", Fault);
        }
    }
}