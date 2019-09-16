// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.MessageData
{
    using System;
    using Metadata;
    using Transformation;
    using Util;


    public static class MessageDataFactory
    {
        public static MessageData<T> Load<T>(IMessageDataRepository repository, Uri address, TransformContext context)
        {
            if (typeof(T) == typeof(string))
                return (MessageData<T>)new LoadMessageData<string>(address, repository, MessageDataConverter.String, context);

            if (typeof(T) == typeof(byte[]))
                return (MessageData<T>)new LoadMessageData<byte[]>(address, repository, MessageDataConverter.ByteArray, context);

            throw new MessageDataException("Unknown message data type: " + TypeMetadataCache<T>.ShortName);
        }
    }
}