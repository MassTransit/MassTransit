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
namespace MassTransit
{
    using System;

    /// <summary>
    /// Specifies the elapsed time before a message expires. When a message expires, the content is no longer
    /// // important and it can be automatically discarded by the message service.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ExpiresInAttribute : Attribute
    {
        private readonly TimeSpan _timeToLive;

        /// <summary>
        /// Specifies the elapsed time before the message expires.
        /// </summary>
        /// <param name="timeToLive">The duration of the time period.</param>
        public ExpiresInAttribute(string timeToLive)
        {
            TimeSpan value;
            if (!TimeSpan.TryParse(timeToLive, out value))
                throw new ArgumentException("Unable to convert string to TimeSpan", "timeToLive");

            _timeToLive = value;
        }

        /// <summary>
        /// Returns the TimeSpan for the message expiration
        /// </summary>
        public TimeSpan TimeToLive
        {
            get { return _timeToLive; }
        }
    }
}