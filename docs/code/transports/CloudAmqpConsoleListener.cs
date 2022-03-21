namespace CloudAmqpConsoleListener;

using System.Security.Authentication;
using System.Threading.Tasks;
using MassTransit;

public class Program
{
    public static async Task Main()
    {
        var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
        {
            cfg.Host("wombat.rmq.cloudamqp.com", 5671, "your_vhost", h =>
            {
                h.Username("your_vhost");
                h.Password("your_password");

                h.UseSsl(s =>
                {
                    s.Protocol = SslProtocols.Tls12;
                });;
            });
        });
    }
}
