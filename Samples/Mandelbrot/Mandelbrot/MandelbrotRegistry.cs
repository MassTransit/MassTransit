namespace Mandelbrot
{
	using Core;
	using MassTransit;
	using MassTransit.Grid;
	using MassTransit.StructureMapIntegration;
	using MassTransit.Transports;
	using StructureMap;

	public class MandelbrotRegistry :
		MassTransitRegistryBase
	{
		public MandelbrotRegistry()
			: base(typeof (LoopbackEndpoint))
		{
			RegisterMandelbrotTypes();

			RegisterControlBus("loopback://localhost/control", control =>{});

			RegisterServiceBus("loopback://localhost/data", x =>
				{
					x.UseControlBus(ObjectFactory.GetInstance<IControlBus>());
				});
		}

		private void RegisterMandelbrotTypes()
		{
			ForRequestedType<SubTaskWorker<GenerateMandelbrotWorker, GenerateRow, RowGenerated>>()
				.TheDefaultIsConcreteType<SubTaskWorker<GenerateMandelbrotWorker, GenerateRow, RowGenerated>>();
		}
	}
}