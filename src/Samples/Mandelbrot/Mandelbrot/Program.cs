namespace Mandelbrot
{
	using System;
	using System.Windows.Forms;
	using log4net;
	using StructureMap;

	internal static class Program
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (Program));

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		private static void Main()
		{
			_log.Info("Starting Mandelbrot Application");

			ObjectFactory.Initialize(x => x.AddRegistry(new MandelbrotRegistry()));

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}
	}
}