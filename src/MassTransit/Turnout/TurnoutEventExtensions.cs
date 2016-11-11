// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Turnout.Contracts;
    using Util;


    public static class TurnoutEventExtensions
    {
        /// <summary>
        /// Returns the arguments from the JobCompleted event
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T GetArguments<T>(this JobCompleted source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return ObjectTypeDeserializer.Deserialize<T>(source.Arguments);
        }

        /// <summary>
        /// Returns the result from the JobCompleted event
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T GetResult<T>(this JobCompleted source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return ObjectTypeDeserializer.Deserialize<T>(source.Results);
        }
    }
}