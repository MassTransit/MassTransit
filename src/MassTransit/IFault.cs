// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Collections.Generic;


    public interface IFault
    {
        /// <summary>
        /// The type of fault that occurred
        /// </summary>
        string FaultType { get; }

        /// <summary>
        /// Messages associated with the exception
        /// </summary>
        List<string> Messages { get; set; }

        /// <summary>
        /// When the exception occurred
        /// </summary>
        DateTime OccurredAt { get; set; }

        /// <summary>
        /// A stack trace related to the exception
        /// </summary>
        List<string> StackTrace { get; set; }
    }
}