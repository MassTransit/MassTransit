// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Transports.RabbitMq
{
    public struct ExchangeBinding
    {
        public readonly string Destination;
        public readonly string Source;
        public readonly bool DestinationTemporary;
        public readonly bool SourceTemporary;

        public ExchangeBinding(string destination, string source, bool destinationTemporary = false, bool sourceTemporary = false)
        {
            Destination = destination;
            Source = source;
            DestinationTemporary = destinationTemporary;
            SourceTemporary = sourceTemporary;
        }

        public bool Equals(ExchangeBinding other)
        {
            return string.Equals(Destination, other.Destination) && string.Equals(Source, other.Source);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            return obj is ExchangeBinding && Equals((ExchangeBinding)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Destination != null
                             ? Destination.GetHashCode()
                             : 0)*397) ^ (Source != null
                                              ? Source.GetHashCode()
                                              : 0);
            }
        }
    }
}