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
namespace MassTransit.Pipeline.Filters
{
    using System.Threading.Tasks;
    using Context;
    using Transformation;
    using Transformation.Contexts;


    /// <summary>
    /// Applies a transform to the message
    /// </summary>
    /// <typeparam name="T">The message type</typeparam>
    public class TransformFilter<T> :
        IFilter<ConsumeContext<T>>
        where T : class
    {
        readonly ITransform<T, T> _transform;

        public TransformFilter(ITransform<T, T> transform)
        {
            _transform = transform;
        }

        Task IFilter<ConsumeContext<T>>.Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
        {
            var transformContext = new ConsumeTransformContext<T>(context);

            TransformResult<T> result = _transform.ApplyTo(transformContext);
            if (result.IsNewValue)
            {
                var transformedContext = new MessageConsumeContext<T>(context, result.Value);

                return next.Send(transformedContext);
            }

            return next.Send(context);
        }

        public bool Visit(IPipelineVisitor visitor)
        {
            return visitor.Visit(this);
        }
    }
}