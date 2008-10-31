namespace MassTransit.Metadata
{
    using System.Reflection;
    using Domain;

    public class MetadataExtracter
    {
        public MessageMetadata Extract<T>(T message)
        {
            var result = new MessageMetadata();
            var messageType = typeof (T);

            result.Name = messageType.Name;
            result.Assembly = messageType.Assembly.GetName().FullName;
            result.Notes = "";
            result.Owner = "";

            foreach (PropertyInfo info in messageType.GetProperties())
            {
                var member = new MemberMetadata
                                 {
                                     Name = info.Name,
                                     ValueType = info.PropertyType.Name,
                                     Notes = ""
                                 };

                //TODO: Needs to walk the entire object graph not just one level deep

                result.Members.Add(member);
            }

            return result;
        }
    }
}