namespace Mandelbrot
{
	using Core;
	using MassTransit.Grid;
	using MassTransit.StructureMapIntegration;
	using MassTransit.Transports;

	public class MandelbrotRegistry :
		MassTransitRegistryBase
	{
		public MandelbrotRegistry(string uriString)
			: base(typeof (LoopbackEndpoint), uriString)
		{
			RegisterMandelbrotTypes();
		}

		private void RegisterMandelbrotTypes()
		{
			ForRequestedType<SubTaskWorker<GenerateMandelbrotWorker, GenerateRow, RowGenerated>>()
				.TheDefaultIsConcreteType<SubTaskWorker<GenerateMandelbrotWorker, GenerateRow, RowGenerated>>();
		}
	}
}