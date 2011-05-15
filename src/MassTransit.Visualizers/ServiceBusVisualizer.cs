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
using System.Diagnostics;
using MassTransit;
using MassTransit.Visualizers;
using Microsoft.VisualStudio.DebuggerVisualizers;

[assembly : DebuggerVisualizer(typeof (ServiceBusVisualizer),
	typeof (VisualizerObjectSource),
	Description = "MT Viz",
	Target = typeof (ServiceBus))]

namespace MassTransit.Visualizers
{
	using System.Text;
	using Microsoft.VisualStudio.DebuggerVisualizers;
	using Pipeline.Inspectors;

	/// <summary>
	/// A Visualizer for ServiceBus.  
	/// </summary>
	public class ServiceBusVisualizer :
		DialogDebuggerVisualizer
	{
		protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
		{
			var bus = (ServiceBus) objectProvider.GetObject();

			using (var displayForm = new ServiceBusForm())
			{
				displayForm.Text = "hi";

				var sb = new StringBuilder();
				sb.AppendLine(string.Format("Listening On: {0}", bus.Endpoint));


				sb.AppendLine("Outbound Pipeline:");
				PipelineViewer.Trace(bus.OutboundPipeline);

				sb.AppendLine("Inbound Pipeline:");
				PipelineViewer.Trace(bus.InboundPipeline);

				displayForm.SetContent(sb.ToString());
				windowService.ShowDialog(displayForm);
			}
		}

		// 
		//    ServiceBusVisualizer.TestShowVisualizer(new SomeType());
		// 
		/// <summary>
		/// Tests the visualizer by hosting it outside of the debugger.
		/// </summary>
		/// <param name="objectToVisualize">The object to display in the visualizer.</param>
		public static void TestShowVisualizer(ServiceBus objectToVisualize)
		{
			var visualizerHost = new VisualizerDevelopmentHost(objectToVisualize, typeof (ServiceBusVisualizer));
			visualizerHost.ShowVisualizer();
		}
	}
}