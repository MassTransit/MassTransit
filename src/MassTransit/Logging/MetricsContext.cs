namespace MassTransit;

using System.Diagnostics;


public interface MetricsContext
{
    void Populate(TagList tagList);
}
