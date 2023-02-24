using MassTransit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit<TestBus>(busConfig =>
{
    busConfig.AddConsumer<TestConsumer>();
    busConfig.AddConsumer<ErrorTestConsumer>();
    busConfig.UsingRabbitMq((context, configurator) =>
    {
        configurator.Host("", "", "", hostConfigurator =>
        {
            hostConfigurator.Username("");
            hostConfigurator.Password("");
        });
    });

});
var app = builder.Build();

app.Run();

public interface TestBus: IBus {
}


public class Message
{
    public string Name { get; set; }
}

public class TestConsumer : IConsumer<Message>
{
    public Task Consume(ConsumeContext<Message> context)
    {
        throw new NotImplementedException();
    }
}

public class ErrorTestConsumer : IConsumer<Message>
{
    public Task Consume(ConsumeContext<Message> context)
    {
        throw new NotImplementedException();
    }
}
