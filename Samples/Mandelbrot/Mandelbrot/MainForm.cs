namespace Mandelbrot
{
	using System;
	using System.ComponentModel;
	using System.Diagnostics;
	using System.Threading;
	using System.Windows.Forms;
	using Core;
	using MassTransit;
	using MassTransit.Grid;
	using StructureMap;

	public partial class MainForm : Form
	{
		private IServiceBus _bus;
		private string _result;
		private BackgroundWorker _worker;

		public MainForm()
		{
			InitializeComponent();
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			_bus = ObjectFactory.GetInstance<IServiceBus>();

			_worker = new BackgroundWorker();
			_worker.DoWork += worker_DoWork;
			_worker.RunWorkerCompleted += worker_RunWorkerCompleted;

			_worker.RunWorkerAsync();
		}

		private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			_messageTextBox.AppendText(Environment.NewLine + _result);
		}

		private void worker_DoWork(object sender, DoWorkEventArgs e)
		{
			ImageGenerator generator = new ImageGenerator(1200, 1000);

			Stopwatch generatorStopwatch = Stopwatch.StartNew();

			_bus.Subscribe<SubTaskWorker<GenerateMandelbrotWorker, GenerateRow, RowGenerated>>();

			ManualResetEvent completed = new ManualResetEvent(false);

			GenerateMandelbrotTask task = new GenerateMandelbrotTask(1600, 1200, 1000);
			task.WhenCompleted(x => completed.Set());

			var distributedTaskController =
				new DistributedTaskController<GenerateMandelbrotTask, GenerateRow, RowGenerated>(_bus, ObjectFactory.GetInstance<IEndpointFactory>(), task);

			distributedTaskController.Start();

			if (completed.WaitOne(TimeSpan.FromMinutes(1)))
			{
				generatorStopwatch.Stop();
				_result = string.Format("Width: {0} Height: {1} Elapsed Time: {2}", generator.Width, generator.Height, generatorStopwatch.Elapsed);
			}
			else
			{
				_result = "Timeout waiting for task to complete";
			}
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			_bus.Dispose();
			_bus = null;

			e.Cancel = false;
		}

		private void _refreshButton_Click(object sender, EventArgs e)
		{
			if (_worker.IsBusy)
				return;

			_worker.RunWorkerAsync();
		}
	}
}