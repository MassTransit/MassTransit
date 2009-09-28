namespace MassTransit.Services.Metadata
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Messages;

    public class MetadataExtracter
    {
        
            public MessageDefinition Extract(Type type)
            {
                var result = new MessageDefinition();
                var name = type.FullName;
                var dotNetType = type.FullName;

                foreach (PropertyInfo info in type.GetProperties())
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