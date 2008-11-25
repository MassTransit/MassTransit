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
namespace MassTransit.Timeout
{
    using System;
    using Magnum.Common.Repository;

	public class ScheduledTimeout :
		IAggregateRoot
    {
        private readonly DateTime _expiresAt;
        private readonly Guid _id;

        public ScheduledTimeout(Guid id, DateTime expiresAt)
        {
            _id = id;
            _expiresAt = expiresAt;
        }

        protected ScheduledTimeout()
        {
        }

        public Guid Id
        {
            get { return _id; }
        }

        public DateTime ExpiresAt
        {
            get { return _expiresAt; }
        }
    }
}