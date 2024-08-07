namespace MassTransit;

using System.Collections.Generic;
using System.Diagnostics;


public static class MetricsContextExtensions
{
    public static void AddMetricTags(this PipeContext pipeContext, string key, object value)
    {
        pipeContext.AddOrUpdatePayload(() => new TagListMetricsContext(key, value), e => e.AddTag(key, value));
    }

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

        public void Populate(TagList tagList)
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
