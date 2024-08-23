namespace MassTransit;

using System.Diagnostics;


public interface MetricsContext
{
    void Populate(ref TagList tagList);
}
