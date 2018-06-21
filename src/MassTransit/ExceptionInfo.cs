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
    /// <summary>
    /// An exception information that is serializable
    /// </summary>
    public interface ExceptionInfo
    {
        /// <summary>
        /// The type name of the exception
        /// </summary>
        string ExceptionType { get; }

        /// <summary>
        /// The inner exception if present (also converted to ExceptionInfo)
        /// </summary>
        ExceptionInfo InnerException { get; }

        /// <summary>
        /// The stack trace of the exception site
        /// </summary>
        string StackTrace { get; }

        /// <summary>
        /// The exception message
        /// </summary>
        string Message { get; }

        /// <summary>
        /// The exception source
        /// </summary>
        string Source { get; }
    }
}