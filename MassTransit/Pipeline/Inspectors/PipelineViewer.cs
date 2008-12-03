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
namespace MassTransit.Pipeline.Inspectors
{
	using System.Text;
	using Sinks;

	public class PipelineViewer :
		IPipelineInspector
	{
		private readonly StringBuilder _text = new StringBuilder();
		private int _depth;

		public string Text
		{
			get { return _text.ToString(); }
		}

		public bool Inspect(MessagePipeline element)
		{
			Append("Pipeline");

			_depth = 1;

			return true;
		}

		public bool Inspect<TMessage>(MessageRouter<TMessage> element) where TMessage : class
		{
			Append(string.Format("Router ({0})", typeof (TMessage).FullName));

			_depth++;

			return true;
		}

		public bool Inspect<TMessage>(MessageSink<TMessage> sink) where TMessage : class
		{
			Append(string.Format("Message Sink ({0})", typeof (TMessage).FullName));

			return true;
		}

		public bool Inspect<TInput, TOutput>(MessageTranslator<TInput, TOutput> translator) where TInput : class where TOutput : class, TInput
		{
			if (typeof(TInput) == typeof(object))
				_depth = 2;

			Append(string.Format("Message Translator ({0} to {1})", typeof(TInput).FullName, typeof(TOutput).FullName));

			return true;
		}

		public bool Inspect<TMessage>(IMessageSink<TMessage> element) where TMessage : class
		{
			Append(string.Format("{1} ({0})", typeof (TMessage).FullName, element.GetType().Name));

			return true;
		}

		private void Pad()
		{
			for (int i = 0; i < _depth; i++)
			{
				_text.Append('\t');
			}
		}

		private void Append(string text)
		{
			Pad();

			_text.AppendFormat(text).AppendLine();
		}

		public static void Trace(MessagePipeline pipeline)
		{
			PipelineViewer viewer = new PipelineViewer();

			pipeline.Inspect(viewer);

			System.Diagnostics.Trace.WriteLine(viewer.Text);
		}
	}
}