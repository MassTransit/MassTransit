// Copyright 2007-2008 The Apache Software Foundation.
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
namespace MassTransit.SystemView.Core.ViewModel
{
    using System.Collections.Generic;
    using System.Linq;

    public class TypeNameHelper
    {
        public static string ConverTypeStringToPrettyName(string type)
        {
            var fullType = type.Split(',');
            var typeNameWithoutAssembly = fullType.Length > 0 ? fullType[0] : type;
            var typeNameParts = typeNameWithoutAssembly.Split('.');

            string prettyName = typeNameParts[typeNameParts.Length - 1];

            var innerGenericTypes = type.Split('`');
            if (innerGenericTypes.Length > 1)
            {
                var generics = new Queue<string>(innerGenericTypes.Reverse().Skip(1).Reverse());

                while (generics.Count > 0)
                {
                    var genericTypeIncludingNamespace = generics.Dequeue();
                    var genericTypeParts = genericTypeIncludingNamespace.Split('.');
                    var genericType = genericTypeParts.Length > 0 ? genericTypeParts[genericTypeParts.Length - 1] : genericTypeIncludingNamespace;

                    prettyName = string.Format("{0}<{1}>", genericType, prettyName);
                }
            }

            return prettyName;
        }
    }
}