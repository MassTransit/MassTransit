namespace MassTransit.Serialization
{
    using Newtonsoft.Json.Linq;


    public static class JsonExtensions
    {
        public static void MergeInto(this JContainer left, JToken right)
        {
            foreach (var rightChild in right.Children<JProperty>())
            {
                var rightChildProperty = rightChild;
                var path = string.Empty.Equals(rightChildProperty.Name) || rightChildProperty.Name.Contains(" ")
                    ? $"['{rightChildProperty.Name}']"
                    : rightChildProperty.Name;

                var leftProperty = left.SelectToken(path);
                if (leftProperty == null)
                {
                    // no matching property, just add
                    left.Add(rightChild);
                }
                else
                {
                    var leftObject = leftProperty as JObject;
                    if (leftObject == null)
                    {
                        // replace value
                        var leftParent = (JProperty)leftProperty.Parent;
                        leftParent.Value = rightChildProperty.Value;
                    }
                    else
                        MergeInto(leftObject, rightChildProperty.Value);
                }
            }
        }

        public static JToken Merge(this JToken left, JToken right)
        {
            if (left.Type != JTokenType.Object)
                return right.DeepClone();

            var leftClone = (JContainer)left.DeepClone();

            MergeInto(leftClone, right);

            return leftClone;
        }
    }
}
