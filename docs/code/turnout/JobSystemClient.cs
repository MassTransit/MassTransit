namespace JobSystemClient
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using JobSystem.Jobs;
    using MassTransit;
    using MassTransit.Contracts.JobService;

    public class Program
    {
        public static async Task Main()
        {
            var busControl = Bus.Factory.CreateUsingRabbitMq();

            var source = new CancellationTokenSource(TimeSpan.FromSeconds(10));

            await busControl.StartAsync(source.Token);
            try
            {
                var serviceClient = busControl.CreateServiceClient();

                var requestClient = serviceClient.CreateRequestClient<ConvertVideo>();

                do
                {
                    string value = await Task.Run(() =>
                    {
                        Console.WriteLine("Enter video format (or quit to exit)");
                        Console.Write("> ");
                        return Console.ReadLine();
                    });

                    if("quit".Equals(value, StringComparison.OrdinalIgnoreCase))
                        break;

                    var response = await requestClient.GetResponse<JobSubmissionAccepted>(new
                    {
                        VideoId = NewId.NextGuid(),
                        Format = value
                    });
                }
                while (true);
            }
            finally
            {
                await busControl.StopAsync();
            }
        }
    }
}
