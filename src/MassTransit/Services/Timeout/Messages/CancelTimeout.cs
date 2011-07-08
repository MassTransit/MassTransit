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
namespace MassTransit.Services.Timeout.Messages
{
    using System;

	/// <summary>
	/// Used by the Timeout Service, cancels the scheduled timeout with the specified CorrelationId
	/// </summary>
    [Serializable]
    public class CancelTimeout :
        CorrelatedBy<Guid>
    {
        /*
        /// <summary>
        /// Initializes a new instance of the <see cref="CancelTimeout"/> class.
        /// </summary>
	    protected CancelTimeout()
	    {
	    }

        /// <summary>
        /// Initializes a new instance of the <see cref="CancelTimeout"/> class.
        /// </summary>
        /// <param name="correlationId">The correlation id.</param>
	    public CancelTimeout(Guid correlationId)
	    {
	        CorrelationId = correlationId;
	    }

        /// <summary>
        /// Initializes a new instance of the <see cref="CancelTimeout"/> class.
        /// </summary>
        /// <param name="correlationId">The correlation id.</param>
        /// <param name="tag">The tag.</param>
	    public CancelTimeout(Guid correlationId, int tag)
	    {
	        CorrelationId = correlationId;
	        Tag = tag;
	    }
        */
        /// <summary>
        /// Gets the correlation id.
        /// </summary>
	    public Guid CorrelationId { get; set; }

        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        /// <value>
        /// The tag.
        /// </value>
        public int Tag { get; set; }
    }
}