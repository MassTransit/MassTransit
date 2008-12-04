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
namespace MassTransit.Pipeline.Sinks
{
	using System;
	using System.Collections.Generic;

	public class MessageFilter<TMessage> :
		MessageSinkBase<TMessage, TMessage> where TMessage : class
	{
		private readonly Func<TMessage, bool> _allow;
        public string Description { get; private set; }

		public MessageFilter(Func<IMessageSink<TMessage>, IMessageSink<TMessage>> insertAfter, Func<TMessage, bool> allow) :
			this("", insertAfter, allow)
		{

		}

        public MessageFilter(string description, Func<IMessageSink<TMessage>, IMessageSink<TMessage>> insertAfter, Func<TMessage, bool> allow) :
            base(null)
        {
            Description = string.IsNullOrEmpty(description) ? "undescribed" : description;
            _allow = allow;

            ReplaceOutputSink(insertAfter(this));
        }

		public override IEnumerable<Consumes<TMessage>.All> Enumerate(TMessage message)
		{
			if (!_allow(message))
				yield break;

			foreach (Consumes<TMessage>.All consumer in _outputSink.ReadLock(x => x.Enumerate(message)))
			{
				yield return consumer;
			}
		}

		public override bool Inspect(IPipelineInspector inspector)
		{
			return inspector.Inspect(this) && _outputSink.ReadLock(x => x.Inspect(inspector));
		}
	}
}