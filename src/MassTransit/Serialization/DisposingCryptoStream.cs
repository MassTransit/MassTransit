// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Serialization
{
    using System.IO;
    using System.Security.Cryptography;

    class DisposingCryptoStream :
        CryptoStream
    {
        Stream _stream;
        ICryptoTransform _transform;

        internal DisposingCryptoStream(Stream stream, ICryptoTransform transform, CryptoStreamMode mode)
            : base(stream, transform, mode)
        {
            _stream = stream;
            _transform = transform;
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            base.Dispose(true);

            if (_stream != null)
            {
                _stream.Dispose();
                _stream = null;
            }

            if (_transform != null)
            {
                _transform.Dispose();
                _transform = null;
            }
        }
    }
}