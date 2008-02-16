namespace MassTransit.Host
{
	public interface IConfigurator
	{
		string File { get; }
		void Configure();
	}
}