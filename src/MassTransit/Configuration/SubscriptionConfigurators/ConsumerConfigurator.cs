// Copyright 2007-2011 The Apache Software Foundation.
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

    public interface ConsumerConfigurator
    {
    }

    public class ConsumerConfiguratorImpl : 
        ConsumerConfigurator
    {
        public ConsumerLifeCycle Lifecycle { get; set; }
        
    }

    public enum ConsumerLifeCycle
    {
        Transient,
        Permanent
    }

    public enum ConsumerType
    {
        All,
        Selected,
        Correlated
    }

    public static class ConsumerConfiguratorExtensions
    {
        public static void Selected<TMessage>(this ConsumerConfigurator cfg, Predicate<TMessage> pred)
        {
            //set the predicate
            //set to selected?
        }

        public static void Permanent(this ConsumerConfigurator cfg)
        {
            //set lifecyle to permanent
        }
    }
}