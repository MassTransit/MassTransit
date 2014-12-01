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
namespace MassTransit.Configurators
{
    using System;
    using System.Collections.Generic;


    public class BusFactory :
        IBusFactory
    {
        public IEnumerable<ValidationResult> Validate()
        {
            yield return this.Failure("Transport", "must be configured using a transport-specific extension method");
        }

        public IBusControl CreateBus()
        {
            throw new NotSupportedException(
                "To create a bus, you must use one of the extension methods to create a bus using the transport for that extension method. To create an in-memory bus, use the CreateUsingInMemory extension method.");
        }
    }
}