namespace MassTransit;

using System.Collections.Generic;
using System.Diagnostics;


public static class MetricsContextExtensions
{
    /// <summary>
    /// Add custom tag to the metrics emitted by the library
    /// </summary>
    /// <param name="pipeContext"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public static void AddMetricTags(this PipeContext pipeContext, string key, object value)
    {
        pipeContext.AddOrUpdatePayload(() => new TagListMetricsContext(key, value), e => e.AddTag(key, value));
    }

    /// <summary>
    /// Set and override custom metric tags emitted by the library
    /// </summary>
    /// <param name="pipeContext"></param>
    /// <param name="tagList"></param>
    public static void SetMetricTags(this PipeContext pipeContext, TagList tagList)
    {
        pipeContext.AddOrUpdatePayload(() => new TagListMetricsContext(tagList), _ => new TagListMetricsContext(tagList));
    }


    class TagListMetricsContext :
        MetricsContext
    {
        TagList _tagList;

        public TagListMetricsContext(string key, object value)
            : this(new TagList { { key, value } })
        {
        }

        public TagListMetricsContext(TagList tagList)
        {
            _tagList = tagList;
        }

        public void Populate(ref TagList tagList)
        {
            foreach (KeyValuePair<string, object> tag in _tagList)
                tagList.Add(tag);
        }

        public TagListMetricsContext AddTag(string key, object value)
        {
            _tagList.Add(key, value);
            return this;
        }
    }
}
