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
namespace MassTransit
{
    using System;
    using System.Linq.Expressions;
    using Metadata;
    using Util;


    [Serializable]
    public class SagaException :
        MassTransitException
    {
        readonly Guid _correlationId;
        readonly Type _messageType;
        readonly Type _sagaType;

        protected SagaException()
        {
        }

        public SagaException(string message, Type sagaType, Type messageType, Guid correlationId)
            : base(FormatMessage(sagaType, correlationId, messageType, message))
        {
            _sagaType = sagaType;
            _messageType = messageType;
            _correlationId = correlationId;
        }

        public SagaException(string message, Type sagaType, Type messageType, Expression findExpression)
            : base($"{sagaType.FullName} {message}({messageType.FullName}) - {findExpression}")
        {
            _sagaType = sagaType;
            _messageType = messageType;
        }

        public SagaException(string message, Type sagaType, Type messageType, Expression findExpression, Exception innerException)
            : base($"{sagaType.FullName} {message}({messageType.FullName}) - {findExpression}", innerException)
        {
            _sagaType = sagaType;
            _messageType = messageType;
        }

        public SagaException(string message, Type sagaType, Type messageType, Guid correlationId, Exception innerException)
            : base(FormatMessage(sagaType, correlationId, messageType, message), innerException)
        {
            _sagaType = sagaType;
            _messageType = messageType;
            _correlationId = correlationId;
        }

        public SagaException(string message, Type sagaType, Type messageType)
            : base(FormatMessage(sagaType, messageType, message))
        {
            _sagaType = sagaType;
            _messageType = messageType;
            _correlationId = Guid.Empty;
        }

        public Type SagaType => _sagaType;

        public Type MessageType => _messageType;

        public Guid CorrelationId => _correlationId;

        static string FormatMessage(Type sagaType, Type messageType, string message)
        {
            return $"{TypeMetadataCache.GetShortName(sagaType)} Saga exception on receipt of {TypeMetadataCache.GetShortName(messageType)}: {message}";
        }

        static string FormatMessage(Type sagaType, Guid correlationId, Type messageType, string message)
        {
            return
                $"{TypeMetadataCache.GetShortName(sagaType)}({correlationId}) Saga exception on receipt of {TypeMetadataCache.GetShortName(messageType)}: {message}";
        }
    }
}