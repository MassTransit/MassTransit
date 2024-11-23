namespace MassTransit.Tests.ContainerTests;

using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;


[TestFixture]
public class EmptyBodyConfiguration_Specs
{
    [Test]
    public void Should_throw_when_no_bus_is_configured()
    {
        var services = new ServiceCollection();

        Assert.Throws<ConfigurationException>(() =>
            services.AddMassTransit(x => { }));
    }

    [Test]
    public void Should_throw_when_no_bus_is_configured_multibus()
    {
        var services = new ServiceCollection();
        Assert.Throws(Is.InstanceOf<TargetInvocationException>().And.InnerException.InstanceOf<ConfigurationException>(),
            () => services.AddMassTransit<ISecondBus>(x => { }));
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public interface ISecondBus : IBus;
}
