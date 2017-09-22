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
namespace MassTransit.TestFramework
{
    using System;
    using System.Runtime.Serialization;


    /// <summary>
    /// Thrown in places where it is expected as part of a unit test
    /// </summary>
    [Serializable]
    public class IntentionalTestException :
        Exception
    {
        public IntentionalTestException()
            : this("This exception was thrown intentionally as part of a test")
        {
        }

        public IntentionalTestException(string message)
            : base(message)
        {
        }

        public IntentionalTestException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected IntentionalTestException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}