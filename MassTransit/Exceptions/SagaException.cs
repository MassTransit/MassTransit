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
namespace MassTransit.Exceptions
{
    using System;

    public class SagaException : 
        Exception
    {
        private readonly Guid _correlationId;
        private readonly Type _messageType;
        private readonly Type _sagaType;

        public SagaException(string message, Type sagaType, Type messageType, Guid correlationId)
            : base(string.Format("{0}({1}) - Unable to load saga on receipt of {2}: {3}", sagaType.FullName, correlationId, messageType.FullName, message))
        {
            _sagaType = sagaType;
            _messageType = messageType;
            _correlationId = correlationId;
        }

        public Type SagaType
        {
            get { return _sagaType; }
        }

        public Type MessageType
        {
            get { return _messageType; }
        }

        public Guid CorrelationId
        {
            get { return _correlationId; }
        }
    }
}