// Copyright 2007-2019 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Scoping
{
    using System;


    /// <summary>
    /// Captures the <see cref="ConsumeContext"/> for the current message as a scoped provider, so that it can be resolved
    /// by components at runtime (since MS DI doesn't support runtime configuration of scopes)
    /// </summary>
    public class ScopedConsumeContextProvider
    {
        ConsumeContext _context;
        bool _accessed;
        ScopedConsumeContext _marker;

        public void SetContext(ConsumeContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            lock (this)
            {
                if (_context == null)
                {
                    _context = context;
                    _marker = new ScopedConsumeContext();

                    context.GetOrAddPayload(() => _marker);
                }
                else if (ReferenceEquals(_context, context))
                {
                }
                else if (!context.TryGetPayload<ScopedConsumeContext>(out _))
                {
                    throw new InvalidOperationException("The ConsumeContext was already set.");
                }
            }
        }

        public ConsumeContext GetContext()
        {
            if (_accessed)
                throw new InvalidOperationException("The ConsumeContext was already accessed.");

            if (_context == null)
                return null;

            _accessed = true;

            return _context;
        }


        class ScopedConsumeContext
        {
        }
    }
}
