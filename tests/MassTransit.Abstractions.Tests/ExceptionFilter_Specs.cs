namespace MassTransit.Abstractions.Tests;

using System;
using Configuration;
using NUnit.Framework;


[TestFixture]
public class ExceptionFilter_Specs
{
    [Test]
    public void Should_match_exception_type()
    {
        var filter = new Filter();

        filter.Handle<Exception>();

        Assert.That(filter.Match(new Exception()), Is.True);
    }

    [Test]
    public void Should_match_exception_type_with_filter()
    {
        var filter = new Filter();

        filter.Handle<Exception>(x => x.Message.Contains("conflict"));

        Assert.That(filter.Match(new Exception("There was a conflict")), Is.True);
    }

    [Test]
    public void Should_not_match_exception_type_with_filter()
    {
        var filter = new Filter();

        filter.Handle<Exception>(x => x.Message.Contains("confusion"));

        Assert.That(filter.Match(new Exception("There was a conflict")), Is.False);
    }

    [Test]
    public void Should_match_base_exception_type_with_filter()
    {
        var filter = new Filter();

        filter.Handle<Exception>(x => x.Message.Contains("conflict"));

        Assert.That(filter.Match(new ArgumentNullException("There was a conflict")), Is.True);
    }

    [Test]
    public void Should_include_the_parameter_name_in_the_exception()
    {
        var exception = new ArgumentNullException("There was a conflict");

        Assert.That(exception.Message, Does.Contain("conflict"));
    }


    class Filter :
        ExceptionSpecification
    {
        public bool Match<T>(T exception)
            where T : Exception
        {
            return Filter.Match(exception);
        }
    }
}
