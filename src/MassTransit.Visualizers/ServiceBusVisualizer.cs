using System.Diagnostics;
using MassTransit;
using MassTransit.Visualizers;
using Microsoft.VisualStudio.DebuggerVisualizers;

[assembly: DebuggerVisualizer(typeof(ServiceBusVisualizer),
    typeof(VisualizerObjectSource),
    Description = "MT Viz",
    Target = typeof(ServiceBus))]

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

        // TODO: Add the following to your testing code to test the visualizer:
        // 
        //    ServiceBusVisualizer.TestShowVisualizer(new SomeType());
        // 
        /// <summary>
        /// Tests the visualizer by hosting it outside of the debugger.
        /// </summary>
        /// <param name="objectToVisualize">The object to display in the visualizer.</param>
        public static void TestShowVisualizer(ServiceBus objectToVisualize)
        {
            var visualizerHost = new VisualizerDevelopmentHost(objectToVisualize, typeof(ServiceBusVisualizer));
            visualizerHost.ShowVisualizer();
        }
    }
}