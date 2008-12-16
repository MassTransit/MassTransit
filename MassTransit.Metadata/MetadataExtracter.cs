namespace MassTransit.Metadata
{
    using System.Collections.Generic;
    using System.Reflection;
    using Messages;

    public class MetadataExtracter
    {
        public class MetadataExtractor
        {
            public Metadata Extract<T>()
            {
                var result = new Metadata();
                var name = typeof(T).FullName;
                var dotNetType = typeof(T).FullName;
                var children = new List<Metadata>();

                foreach (PropertyInfo info in typeof(T).GetProperties())
                {
                    var member = new Metadata()
                    {
                        Name = info.Name,
                        DotNetType = info.PropertyType.Name,
                        Owner = "",
                        Notes = "",
                        Parent = result,
                        Children = new List<Metadata>()
                    };

                    //TODO: Needs to walk the entire object graph not just one level deep

                    children.Add(member);
                }

                result.Name = name;
                result.DotNetType = dotNetType;
                result.Children = children;
                result.Owner = "";
                result.Notes = "";

                return result;
            }
        }
    }
}