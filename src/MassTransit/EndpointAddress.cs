// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Magnum;
    using Util;

    public class EndpointAddress :
        IEndpointAddress
    {
        protected static readonly string LocalMachineName = Environment.MachineName.ToLowerInvariant();
        static IEndpointAddress _null;
        Func<bool> _isLocal;
        bool _isTransactional;
        Uri _uri;

        public EndpointAddress(Uri uri)
        {
            Guard.AgainstNull(uri, "uri");

            _uri = uri;

            _isLocal = () => DetermineIfEndpointIsLocal(uri);

            _isTransactional = CheckForTransactionalHint(uri, false);
        }

        public EndpointAddress(string uriString)
        {
            Guard.AgainstEmpty(uriString, "uriString");

            try
            {
                _uri = new Uri(uriString);
            }
            catch (UriFormatException ex)
            {
                throw new ArgumentException("The URI is invalid: " + uriString, ex);
            }

            _isLocal = () => DetermineIfEndpointIsLocal(_uri);

            _isTransactional = CheckForTransactionalHint(_uri, false);
        }

        public static IEndpointAddress Null
        {
            get { return _null ?? (_null = new EndpointAddress(new Uri("null://null/null"))); }
        }

        public Uri Uri
        {
            get { return _uri; }
            protected set { _uri = value; }
        }

        public bool IsLocal
        {
            get { return _isLocal(); }
        }

        public string Path
        {
            get { return Uri.AbsolutePath.Substring(1); }
        }

        public bool IsTransactional
        {
            get { return _isTransactional; }
            protected set { _isTransactional = value; }
        }

        public override string ToString()
        {
            return _uri.ToString();
        }

        protected virtual bool DetermineIfEndpointIsLocal(Uri uri)
        {
            string hostName = uri.Host;
            bool local = string.Compare(hostName, ".") == 0 ||
                         string.Compare(hostName, "localhost", true) == 0 ||
                         string.Compare(uri.Host, LocalMachineName, true) == 0;

            Interlocked.Exchange(ref _isLocal, () => local);

            return local;
        }

        protected static bool CheckForTransactionalHint(Uri uri, bool defaultTransactional)
        {
            return uri.Query.GetValueFromQueryString("tx", defaultTransactional);
        }
    }
}