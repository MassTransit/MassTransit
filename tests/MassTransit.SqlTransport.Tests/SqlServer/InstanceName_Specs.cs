namespace MassTransit.DbTransport.Tests.SqlServer;

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using SqlTransport.SqlServer;
using Testing;


[TestFixture]
public class InstanceName_Specs
{
    [Test]
    public async Task Should_include_the_instance_name()
    {
        await using var provider = new ServiceCollection()
            .AddMassTransitTestHarness(x =>
            {
                x.AddOptions<SqlTransportOptions>().Configure(options =>
                {
                    options.Host = "localhost\\instance";
                    options.Database = "masstransit_transport_tests";
                    options.Schema = "transport";
                    options.Role = "transport";
                    options.Username = "unit_tests";
                    options.Password = "H4rd2Gu3ss!";
                    options.AdminUsername = "sa";
                    options.AdminPassword = "Password12!";
                });

                x.UsingSqlServer();
            })
            .BuildServiceProvider(true);

        var connection = SqlServerSqlTransportConnection.GetDatabaseConnection(provider.GetRequiredService<IOptions<SqlTransportOptions>>().Value);

        Assert.That(connection.Connection.ConnectionString, Contains.Substring("Data Source=localhost\\instance"));
    }

    [Test]
    public async Task Should_include_the_instance_name_and_port_and_start()
    {
        await using var provider = new ServiceCollection()
            .AddMassTransitTestHarness(x =>
            {
                x.AddOptions<SqlTransportOptions>().Configure(options =>
                {
                    options.Host = "localhost\\instance";
                    options.Port = 3381;
                    options.Database = "masstransit_transport_tests";
                    options.Schema = "transport";
                    options.Role = "transport";
                    options.Username = "unit_tests";
                    options.Password = "H4rd2Gu3ss!";
                    options.AdminUsername = "sa";
                    options.AdminPassword = "Password12!";
                });

                x.UsingSqlServer();
            })
            .BuildServiceProvider(true);

        var harness = await provider.StartTestHarness();

        Assert.Multiple(() =>
        {
            Assert.That(harness.Bus.Address.Host, Is.EqualTo("localhost"));
            Assert.That(harness.Bus.Address.Port, Is.EqualTo(3381));
            Assert.That(harness.Bus.Address.Query, Is.EqualTo("?autodelete=300&instance=instance"));
        });

        var connection = SqlServerSqlTransportConnection.GetDatabaseConnection(provider.GetRequiredService<IOptions<SqlTransportOptions>>().Value);

        Assert.That(connection.Connection.ConnectionString, Contains.Substring("Data Source=localhost\\instance,3381"));
    }

    [Test]
    public async Task Should_include_the_instance_name_and_start()
    {
        await using var provider = new ServiceCollection()
            .AddMassTransitTestHarness(x =>
            {
                x.AddOptions<SqlTransportOptions>().Configure(options =>
                {
                    options.Host = "localhost\\instance";
                    options.Database = "masstransit_transport_tests";
                    options.Schema = "transport";
                    options.Role = "transport";
                    options.Username = "unit_tests";
                    options.Password = "H4rd2Gu3ss!";
                    options.AdminUsername = "sa";
                    options.AdminPassword = "Password12!";
                });

                x.UsingSqlServer();
            })
            .BuildServiceProvider(true);

        var harness = await provider.StartTestHarness();

        Assert.Multiple(() =>
        {
            Assert.That(harness.Bus.Address.GetLeftPart(UriPartial.Authority), Is.EqualTo("db://localhost"));
            Assert.That(harness.Bus.Address.Query, Is.EqualTo("?autodelete=300&instance=instance"));
        });
    }
}
