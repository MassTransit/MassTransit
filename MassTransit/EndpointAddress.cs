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
namespace MassTransit
{
    using System;
    using System.Threading;

    public class EndpointAddress :
        IEndpointAddress
    {
        private static readonly IEndpointAddress _nullEndpointAddress = new EndpointAddress(new Uri("null://nul/nul"));
        protected static readonly string LocalMachineName = Environment.MachineName.ToLowerInvariant();

        private Func<bool> _isLocal;

        public EndpointAddress(Uri uri)
        {
            Uri = uri;
            _isLocal = () => DetermineIfEndpointIsLocal(uri);
        }

        public static IEndpointAddress Null
        {
            get { return _nullEndpointAddress; }
        }

        public Uri Uri { get; protected set; }

        public bool IsLocal
        {
            get { return _isLocal(); }
        }

        public string Path
        {
            get { return Uri.AbsolutePath.Substring(1); }
        }

        public override string ToString()
        {
            return Uri.ToString();
        }

        private bool DetermineIfEndpointIsLocal(Uri uri)
        {
            string hostName = uri.Host;
            bool local = string.Compare(hostName, ".") == 0 ||
                         string.Compare(hostName, "localhost", true) == 0 ||
                         string.Compare(uri.Host, LocalMachineName, true) == 0;

            Interlocked.Exchange(ref _isLocal, () => local);

            return local;
        }
    }
}