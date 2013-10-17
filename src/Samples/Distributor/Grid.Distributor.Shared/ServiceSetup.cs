namespace Grid.Distributor.Shared
{
    using Topshelf;


    public abstract class ServiceSetup
    {
        public abstract string ServiceName { get; set; }
        public abstract string DisplayName { get; set; }
        public abstract string Description { get; set; }
        public abstract string SourceQueue { get; set; }

        public void ConfigureService<T>()
            where T : class, IServiceInterface, new()
        {
            HostFactory.Run(c =>
                {
                    c.SetServiceName(ServiceName);
                    c.SetDisplayName(DisplayName);
                    c.SetDescription(Description);

                    c.RunAsLocalSystem();

                    c.Service<T>(s =>
                        {
                            s.ConstructUsing(name => new T());

                            s.WhenStarted(start => start.Start());
                            s.WhenStopped(stop => stop.Stop());
                        });
                });
        }
    }
}