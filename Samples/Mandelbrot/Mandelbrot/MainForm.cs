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
		private GenerateMandelbrotTask _task;
		private UnsubscribeAction _unsubscribe;
		private double _zoomRate = 1.5;

		public MainForm()
		{
			InitializeComponent();
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			_bus = ObjectFactory.GetInstance<IServiceBus>();
			_unsubscribe = _bus.Subscribe<SubTaskWorker<GenerateMandelbrotWorker, GenerateRow, RowGenerated>>();

			_worker = new BackgroundWorker();
			_worker.DoWork += worker_DoWork;
			_worker.RunWorkerCompleted += worker_RunWorkerCompleted;

			var task = new GenerateMandelbrotTask(_viewer.Width, _viewer.Height, 1000);

			_worker.RunWorkerAsync(task);
		}

		private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			_messageTextBox.AppendText(Environment.NewLine + _result);

			try
			{
				_viewer.UpdateDisplayWithResults(_task);
			}
			catch (Exception ex)
			{
			}
		}

		private void worker_DoWork(object sender, DoWorkEventArgs e)
		{
			GenerateMandelbrotTask task = e.Argument as GenerateMandelbrotTask;
			if (task == null)
				return;

			var generatorStopwatch = Stopwatch.StartNew();

			var completed = new ManualResetEvent(false);

			task.WhenCompleted(x => completed.Set());

			var distributedTaskController =
				new DistributedTaskController<GenerateMandelbrotTask, GenerateRow, RowGenerated>(_bus, ObjectFactory.GetInstance<IEndpointFactory>(), task);

			distributedTaskController.Start();

			if (completed.WaitOne(TimeSpan.FromMinutes(1)))
			{
				generatorStopwatch.Stop();
				_result = string.Format("Width: {0} Height: {1} Elapsed Time: {2}", task.Width, task.Height, generatorStopwatch.Elapsed);
				_task = task;
			}
			else
			{
				_result = "Timeout waiting for task to complete";
			}
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			_unsubscribe();

			_bus.Dispose();
			_bus = null;

			e.Cancel = false;
		}

		private void refreshView_Click(object sender, EventArgs e)
		{
			if (_worker.IsBusy)
				return;

			var task = new GenerateMandelbrotTask(_viewer.Width, _viewer.Height, 1000);

			_worker.RunWorkerAsync(task);
		}

		private void _viewer_MouseDown(object sender, MouseEventArgs e)
		{
			if (_worker.IsBusy)
				return;

			if (_task == null)
				return;
            
			// pixel that was clicked in coordinates
			double x = (e.X * _task.SampleWidth) / _task.Width + _task.OffsetX;
			double y = (e.Y * _task.SampleHeight) / _task.Height + _task.OffsetY;

			double sampleWidth = _task.SampleWidth*(1/_zoomRate);
			double sampleHeight = sampleWidth * _task.SampleHeight / _task.SampleWidth;

			double offsetX = x - (sampleWidth) / 2;
			double offsetY = y - (sampleHeight) / 2;

			var task = new GenerateMandelbrotTask(_viewer.Width, _viewer.Height, 1000, offsetX, offsetY, sampleWidth, sampleHeight);

			_worker.RunWorkerAsync(task);
		}

		private void _zoom150_Click(object sender, EventArgs e)
		{
			_zoomRate = 1.5;
			_zoom150.Checked = true;
			_zoom200.Checked = false;
			_zoom400.Checked = false;
			_zoom800.Checked = false;
		}

		private void _zoom200_Click(object sender, EventArgs e)
		{
			_zoomRate = 2;
			_zoom150.Checked = false;
			_zoom200.Checked = true;
			_zoom400.Checked = false;
			_zoom800.Checked = false;
		}

		private void _zoom400_Click(object sender, EventArgs e)
		{
			_zoomRate = 4;
			_zoom150.Checked = false;
			_zoom200.Checked = false;
			_zoom400.Checked = true;
			_zoom800.Checked = false;
		}

		private void _zoom800_Click(object sender, EventArgs e)
		{
			_zoomRate = 8;
			_zoom150.Checked = false;
			_zoom200.Checked = false;
			_zoom400.Checked = false;
			_zoom800.Checked = true;
		}
	}
}