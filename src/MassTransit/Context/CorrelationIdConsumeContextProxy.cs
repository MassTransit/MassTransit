// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Context
{
    using System;


    /// <summary>
    /// A consumer instance merged with a message consume context
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public class CorrelationIdConsumeContextProxy<TMessage> :
        ConsumeContextProxy<TMessage>
        where TMessage : class
    {
        readonly Guid _correlationId;

        public CorrelationIdConsumeContextProxy(ConsumeContext<TMessage> context, Guid correlationId)
            : base(context)
        {
            _correlationId = correlationId;
        }

        public override Guid? CorrelationId => _correlationId;
    }
}