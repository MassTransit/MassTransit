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

using System;

namespace MassTransit.ServiceBus.Util
{
    public class MessageId : IEquatable<MessageId>
    {
        private static readonly MessageId _empty = new MessageId(Guid.Empty, 0);

        private readonly Guid _id;
        private readonly int _sequence;

        protected MessageId(Guid id, int sequence)
        {
            _id = id;
            _sequence = sequence;
        }

        public MessageId()
        {
            _id = _empty.Id;
            _sequence = _empty.Sequence;
        }

        public MessageId(MessageId other)
        {
            _id = other._id;
            _sequence = other._sequence;
        }

        public MessageId(string s)
            : this()
        {
            //TODO: Use regex?
            string[] parts = s.Split('\\');
            if (parts.Length == 2)
            {
                _id = new Guid(parts[0]);
                _sequence = int.Parse(parts[1]);
            }
            else
            {
                throw new Exception(string.Format("MessageId is in the wrong format. Should be like '00000000-0000-0000-0000-000000000000\\0' not '{0}'", s));
            }
        }

        public Guid Id
        {
            get { return _id; }
        }

        public int Sequence
        {
            get { return _sequence; }
        }

        public static MessageId Empty
        {
            get { return _empty; }
        }

        #region IEquatable<MessageId> Members

        public bool Equals(MessageId other)
        {
            if (_id != other._id)
                return false;

            if (_sequence != other._sequence)
                return false;

            return true;
        }

        #endregion

        public override string ToString()
        {
            return string.Format(@"{0}\{1}", _id, _sequence);
        }

        public override bool Equals(object obj)
        {
            if (obj is MessageId)
                return Equals((MessageId) obj);

            return false;
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode() + 29*_sequence.GetHashCode();
        }

        public static implicit operator MessageId(string id)
        {
            return new MessageId(id);
        }

        public static implicit operator string(MessageId id)
        {
            return id.ToString();
        }

        public static bool operator ==(MessageId leftHandValue, MessageId rightHandValue)
        {
            return leftHandValue.Equals(rightHandValue);
        }

        public static bool operator !=(MessageId leftHandValue, MessageId rightHandValue)
        {
            return !leftHandValue.Equals(rightHandValue);
        }
    }
}