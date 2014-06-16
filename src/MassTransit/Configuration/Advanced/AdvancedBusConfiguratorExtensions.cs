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
namespace MassTransit.Advanced
{
	using System;
	using System.Collections.Generic;


    public static class AdvancedBusConfiguratorExtensions
	{
	    public static UnsubscribeAction CombineSubscriptions<T>(this IEnumerable<T> source, Func<T, UnsubscribeAction> map)
	    {
	        UnsubscribeAction result = null;
	        foreach (T item in source)
	        {
	            UnsubscribeAction unsubscribeAction = map(item);
	            if (result == null)
	                result = unsubscribeAction;
	            else
	            {
	                UnsubscribeAction previousResult = result;
	                result = () => previousResult() && unsubscribeAction();
	            }
	        }
	        return result ?? (() => true);
	    }
	}
}