// Copyright 2007-2013 Chris Patterson
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.Courier.Serialization
{
    using Newtonsoft.Json.Linq;


    public static class JsonExtensions
    {
        public static void MergeInto(this JContainer left, JToken right)
        {
            foreach (JProperty rightChild in right.Children<JProperty>())
            {
                JProperty rightChildProperty = rightChild;
                var path = (string.Empty.Equals(rightChildProperty.Name) || rightChildProperty.Name.Contains(" ") ? $"['{rightChildProperty.Name}']" :
               rightChildProperty.Name);
                JToken leftProperty = left.SelectToken(path);
                //JToken leftProperty = left.SelectToken(rightChildProperty.Name);

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
                        // recurse object
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