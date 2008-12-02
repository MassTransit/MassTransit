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
namespace MassTransit.Pipeline
{
	/// <summary>
	/// Base class for pipeline inspectors
	/// </summary>
	public class PipelineInspectorBase :
		IPipelineInspector
	{
		public virtual bool Inspect(MessagePipeline element)
		{
			return true;
		}

		public virtual bool Inspect<TMessage>(MessageRouter<TMessage> element) where TMessage : class
		{
			return true;
		}

		public virtual bool Inspect<TMessage>(MessageSink<TMessage> sink) where TMessage : class
		{
			return true;
		}

		public virtual bool Inspect<TInput, TOutput>(MessageTranslator<TInput, TOutput> translator)
			where TInput : class
			where TOutput : class, TInput
		{
			return true;
		}

		public virtual bool Inspect<TMessage>(IMessageSink<TMessage> element) where TMessage : class
		{
			return true;
		}

//		public bool Inspect(object obj)
//		{
//			Type t = obj.GetType();
//
//			Type myT = GetType();
//
//			MethodInfo mi = myT.GetMethod("Inspect", new Type[] { t });
//			if (mi != null)
//			{
//				mi.Invoke(this, new object[] { obj });
//				return;
//			}
//
//			if (t.IsAssignableFrom(typeof(ISegment)))
//			{
//				Inspect((ISegment) obj);
//				return;
//			}
//
//			throw new ArgumentException("Don't know how to format object " + obj);
//		}
	}
}