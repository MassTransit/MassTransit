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
    using System.Collections.Generic;
    using Magnum.Extensions;


    /// <summary>
    /// A fault is a system-generated message that is published when an exception occurs while processing a message.
    /// </summary>
    /// <typeparam name="TMessage">The type of message that threw the exception</typeparam>
    [Serializable]
    public class Fault<TMessage> :
        IFault
        where TMessage : class
    {
        /// <summary>
        /// Creates a new fault message for the failed message
        /// </summary>
        /// <param name="ex">The exception thrown by the message consumer</param>
        /// <param name="message">The message that was being processed when the exception was thrown</param>
        public Fault(TMessage message, Exception ex)
        {
            FailedMessage = message;
            OccurredAt = DateTime.UtcNow;
            Messages = GetExceptionMessages(ex);
            StackTrace = GetStackTrace(ex);

            FaultType = typeof(Fault<TMessage>).ToShortTypeName();
        }

        protected Fault()
        {
        }

        /// <summary>
        /// The message that failed to be consumed
        /// </summary>
        public TMessage FailedMessage { get; set; }

        public string FaultType { get; set; }

        /// <summary>
        /// Messages associated with the exception
        /// </summary>
        public List<string> Messages { get; set; }

        /// <summary>
        /// When the exception occurred
        /// </summary>
        public DateTime OccurredAt { get; set; }

        /// <summary>
        /// A stack trace related to the exception
        /// </summary>
        public List<string> StackTrace { get; set; }

        static List<string> GetStackTrace(Exception ex)
        {
            var result = new List<string> {string.IsNullOrEmpty(ex.StackTrace) ? "Stack Trace" : ex.StackTrace};

            Exception innerException = ex.InnerException;
            while (innerException != null)
            {
                string stackTrace = "InnerException Stack Trace: " + innerException.StackTrace;
                result.Add(stackTrace);

                innerException = innerException.InnerException;
            }

            return result;
        }

        static List<string> GetExceptionMessages(Exception ex)
        {
            var result = new List<string> {ex.Message};

            Exception innerException = ex.InnerException;
            while (innerException != null)
            {
                result.Add(innerException.Message);

                innerException = innerException.InnerException;
            }

            return result;
        }
    }

    /// <summary>
    /// A fault is a system-generated message that is published when an exception occurs while processing a message.
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    [Serializable]
    public class Fault<TMessage, TKey> :
        Fault<TMessage>,
        CorrelatedBy<TKey>
        where TMessage : class, CorrelatedBy<TKey>
    {
        /// <summary>
        /// Creates a new Fault message for the failed correlated message
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="message"></param>
        public Fault(TMessage message, Exception ex)
            : base(message, ex)
        {
            CorrelationId = message.CorrelationId;

            FaultType = typeof(Fault<TMessage, TKey>).ToShortTypeName();
        }

        protected Fault()
        {
        }

        public TKey CorrelationId { get; set; }
    }
}