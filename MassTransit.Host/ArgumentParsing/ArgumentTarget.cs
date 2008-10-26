/// Copyright 2007-2008 The Apache Software Foundation.
/// 
/// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
/// this file except in compliance with the License. You may obtain a copy of the 
/// License at 
/// 
///   http://www.apache.org/licenses/LICENSE-2.0 
/// 
/// Unless required by applicable law or agreed to in writing, software distributed 
/// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
/// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
/// specific language governing permissions and limitations under the License.
namespace MassTransit.Host.ArgumentParsing
{
    using System.Reflection;

    public class ArgumentTarget
    {
        private readonly ArgumentAttribute _attribute;
        private readonly PropertyInfo _property;

        public ArgumentTarget(ArgumentAttribute attribute, PropertyInfo property)
        {
            _attribute = attribute;
            _property = property;
        }

        public PropertyInfo Property
        {
            get { return _property; }
        }

        public ArgumentAttribute Attribute
        {
            get { return _attribute; }
        }
    }

    public class DefaultArgumentTarget : ArgumentTarget
    {
        public DefaultArgumentTarget(ArgumentAttribute attribute, PropertyInfo property)
            : base(attribute, property)
        {
        }
    }
}