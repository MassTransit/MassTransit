namespace MassTransit.RabbitMqTransport
{
    using System.Collections.Generic;
    using RabbitMQ.Client;


    class BasicProperties :
        IBasicProperties
    {
        string _appId;
        bool _appIdPresent;
        string _clusterId;
        bool _clusterIdPresent;
        string _contentEncoding;
        bool _contentEncodingPresent;
        string _contentType;

        bool _contentTypePresent;
        string _correlationId;
        bool _correlationIdPresent;
        byte _deliveryMode;
        bool _deliveryModePresent;
        string _expiration;
        bool _expirationPresent;
        IDictionary<string, object> _headers;
        bool _headersPresent;
        string _messageId;
        bool _messageIdPresent;
        byte _priority;
        bool _priorityPresent;
        string _replyTo;
        bool _replyToPresent;
        AmqpTimestamp _timestamp;
        bool _timestampPresent;
        string _type;
        bool _typePresent;
        string _userId;
        bool _userIdPresent;

        public string ContentType
        {
            get => _contentType;
            set
            {
                _contentTypePresent = value != null;
                _contentType = value;
            }
        }

        public string ContentEncoding
        {
            get => _contentEncoding;
            set
            {
                _contentEncodingPresent = value != null;
                _contentEncoding = value;
            }
        }

        public IDictionary<string, object> Headers
        {
            get => _headers;
            set
            {
                _headersPresent = value != null;
                _headers = value;
            }
        }

        public byte DeliveryMode
        {
            get => _deliveryMode;
            set
            {
                _deliveryModePresent = true;
                _deliveryMode = value;
            }
        }

        public bool Persistent
        {
            get => DeliveryMode == 2;
            set => DeliveryMode = value ? (byte)2 : (byte)1;
        }

        public byte Priority
        {
            get => _priority;
            set
            {
                _priorityPresent = true;
                _priority = value;
            }
        }

        public string CorrelationId
        {
            get => _correlationId;
            set
            {
                _correlationIdPresent = value != null;
                _correlationId = value;
            }
        }

        public string ReplyTo
        {
            get => _replyTo;
            set
            {
                _replyToPresent = value != null;
                _replyTo = value;
            }
        }

        public PublicationAddress ReplyToAddress
        {
            get
            {
                PublicationAddress.TryParse(ReplyTo, out var result);
                return result;
            }
            set => ReplyTo = value.ToString();
        }

        public string Expiration
        {
            get => _expiration;
            set
            {
                _expirationPresent = value != null;
                _expiration = value;
            }
        }

        public string MessageId
        {
            get => _messageId;
            set
            {
                _messageIdPresent = value != null;
                _messageId = value;
            }
        }

        public AmqpTimestamp Timestamp
        {
            get => _timestamp;
            set
            {
                _timestampPresent = true;
                _timestamp = value;
            }
        }

        public string Type
        {
            get => _type;
            set
            {
                _typePresent = value != null;
                _type = value;
            }
        }

        public string UserId
        {
            get => _userId;
            set
            {
                _userIdPresent = value != null;
                _userId = value;
            }
        }

        public string AppId
        {
            get => _appId;
            set
            {
                _appIdPresent = value != null;
                _appId = value;
            }
        }

        public string ClusterId
        {
            get => _clusterId;
            set
            {
                _clusterIdPresent = value != null;
                _clusterId = value;
            }
        }

        public void ClearContentType()
        {
            _contentTypePresent = false;
        }

        public void ClearContentEncoding()
        {
            _contentEncodingPresent = false;
        }

        public void ClearHeaders()
        {
            _headersPresent = false;
        }

        public void ClearDeliveryMode()
        {
            _deliveryModePresent = false;
        }

        public void ClearPriority()
        {
            _priorityPresent = false;
        }

        public void ClearCorrelationId()
        {
            _correlationIdPresent = false;
        }

        public void ClearReplyTo()
        {
            _replyToPresent = false;
        }

        public void ClearExpiration()
        {
            _expirationPresent = false;
        }

        public void ClearMessageId()
        {
            _messageIdPresent = false;
        }

        public void ClearTimestamp()
        {
            _timestampPresent = false;
        }

        public void ClearType()
        {
            _typePresent = false;
        }

        public void ClearUserId()
        {
            _userIdPresent = false;
        }

        public void ClearAppId()
        {
            _appIdPresent = false;
        }

        public void ClearClusterId()
        {
            _clusterIdPresent = false;
        }

        public bool IsContentTypePresent()
        {
            return _contentTypePresent;
        }

        public bool IsContentEncodingPresent()
        {
            return _contentEncodingPresent;
        }

        public bool IsHeadersPresent()
        {
            return _headersPresent;
        }

        public bool IsDeliveryModePresent()
        {
            return _deliveryModePresent;
        }

        public bool IsPriorityPresent()
        {
            return _priorityPresent;
        }

        public bool IsCorrelationIdPresent()
        {
            return _correlationIdPresent;
        }

        public bool IsReplyToPresent()
        {
            return _replyToPresent;
        }

        public bool IsExpirationPresent()
        {
            return _expirationPresent;
        }

        public bool IsMessageIdPresent()
        {
            return _messageIdPresent;
        }

        public bool IsTimestampPresent()
        {
            return _timestampPresent;
        }

        public bool IsTypePresent()
        {
            return _typePresent;
        }

        public bool IsUserIdPresent()
        {
            return _userIdPresent;
        }

        public bool IsAppIdPresent()
        {
            return _appIdPresent;
        }

        public bool IsClusterIdPresent()
        {
            return _clusterIdPresent;
        }

        public ushort ProtocolClassId => 60;
        public string ProtocolClassName => "basic";
    }
}
