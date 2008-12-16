namespace MassTransit.Services.Metadata
{
    using System.Collections.Generic;
    using System.Reflection;
    using Messages;

    public class MetadataExtracter
    {
        public class MetadataExtractor
        {
            public MessageDefinition Extract<T>()
            {
                var result = new MessageDefinition();
                var name = typeof(T).FullName;
                var dotNetType = typeof(T).FullName;

                foreach (PropertyInfo info in typeof(T).GetProperties())
                {
                    var member = new MessageDefinition()
                                     {
                                         Name = info.Name,
                                         DotNetType = info.PropertyType.Name,
                                         Parent = result,
                                         Children = new List<MessageDefinition>()
                                     };

                    //TODO: Needs to walk the entire object graph not just one level deep

                    result.Children.Add(member);
                }

                result.Name = name;
                result.DotNetType = dotNetType;

                return result;
            }
        }
    }
}