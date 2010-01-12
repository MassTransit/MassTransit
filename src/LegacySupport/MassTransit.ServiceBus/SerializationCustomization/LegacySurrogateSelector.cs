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
namespace MassTransit.LegacySupport.SerializationCustomization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    public class LegacySurrogateSelector :
        ISurrogateSelector
    {
        readonly SurrogateSelector _inner = new SurrogateSelector();
        readonly List<LegacySurrogate> _surrogates = new List<LegacySurrogate>();

        #region ISurrogateSelector Members

        public void ChainSelector(ISurrogateSelector selector)
        {
            _inner.ChainSelector(selector);
        }

        public ISerializationSurrogate GetSurrogate(Type type, StreamingContext context, out ISurrogateSelector selector)
        {
            if (_surrogates.Any(x => x.SurrogateType == type))
            {
                selector = this;
                return _surrogates.First(x => x.SurrogateType == type);
            }


            return _inner.GetSurrogate(type, context, out selector);
        }

        public ISurrogateSelector GetNextSelector()
        {
            return _inner.GetNextSelector();
        }

        #endregion

        public void AddSurrogate(LegacySurrogate surrogate)
        {
            _surrogates.Add(surrogate);
        }
    }
}