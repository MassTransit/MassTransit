// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Context
{
    using System;
    using System.Linq;
    using GreenPipes.Filters;
    using Internals.Extensions;


    public class PublishContextConverterFactory :
        IPipeContextConverterFactory<PublishContext>
    {
        IPipeContextConverter<PublishContext, TOutput> IPipeContextConverterFactory<PublishContext>.GetConverter<TOutput>()
        {
            var innerType = typeof(TOutput).GetClosingArguments(typeof(PublishContext<>)).Single();

            return (IPipeContextConverter<PublishContext, TOutput>)Activator.CreateInstance(typeof(Converter<>).MakeGenericType(innerType));
        }


        class Converter<T> :
            IPipeContextConverter<PublishContext, PublishContext<T>>
            where T : class
        {
            bool IPipeContextConverter<PublishContext, PublishContext<T>>.TryConvert(PublishContext input, out PublishContext<T> output)
            {
                output = input as PublishContext<T>;

                return output != null;
            }
        }
    }
}