// Copyright 2007-2010 The Apache Software Foundation.
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
namespace MassTransit.Metadata
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Mono.Cecil;
    using Mono.Cecil.Cil;

    public class MetaDataSearcher
    {
        public List<BusMetadata> FindTypesUsingServiceBusPublish()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var assemblyLocations = assemblies.Select(a => a.Location).ToList();


            var result = new List<BusMetadata>();

            foreach (var assemblyLocation in assemblyLocations)
            {
                result.AddRange(ProcessAssemblyLocation(assemblyLocation));
            }

            return result;
        }

        List<BusMetadata> ProcessAssemblyLocation(string assemblyLocation)
        {
            var result = new List<BusMetadata>();

            var assemblyDef = AssemblyFactory.GetAssembly(assemblyLocation);
            if (SkipAssembly(assemblyDef))
                return result;

            foreach (ModuleDefinition module in assemblyDef.Modules)
            {
                result.AddRange(ProcessModule(module));
            }

            return result;
        }

        List<BusMetadata> ProcessModule(ModuleDefinition module)
        {
            var result = new List<BusMetadata>();

            foreach (TypeDefinition type in module.Types)
            {
                result.AddRange(ProcessType(type));
            }

            return result;
        }

        List<BusMetadata> ProcessType(TypeDefinition type)
        {
            var result = new List<BusMetadata>();

            foreach (MethodDefinition method in type.Methods)
            {
                if (!method.HasBody)
                    continue;

                result.AddRange(ProcessMethod(type, method));
            }

            result.ForEach(bd=> bd.UsingType = type.FullName);

            return result;
        }

        List<BusMetadata> ProcessMethod(TypeDefinition type, MethodDefinition method)
        {
            var busUsers = new List<BusMetadata>();

            foreach (Instruction instruction in method.Body.Instructions)
            {
                if (instruction.OpCode.Code != Code.Call && instruction.OpCode.Code != Code.Callvirt)
                    continue;

                var methodRef = instruction.Operand as MethodReference;
                if (methodRef == null)
                    continue;

                if (methodRef.Name != "Publish" && methodRef.Name != "Subscribe")
                    continue;

                //if using the interface
                if (methodRef.DeclaringType.FullName == "MassTransit.IServiceBus")
                {
                    busUsers.Add(ProcessInstruction(instruction));
                    continue;
                }


                //else scan for the interface
                var interfaces = methodRef.DeclaringType.Module.Types[methodRef.DeclaringType.FullName].Interfaces;

                foreach (TypeReference iface in interfaces)
                {
                    if (iface.FullName == "MassTransit.IServiceBus")
                        busUsers.Add(ProcessInstruction(instruction));
                }
            }

            return busUsers;
        }

        BusMetadata ProcessInstruction(Instruction instruction)
        {
            var method = (MethodReference) instruction.Operand;
            if (method.Name == "Publish")
            {
                var gen = (GenericInstanceMethod) method;
                return new BusMetadata()
                           {
                               Direction = Direction.Outbound,
                               MessageType = gen.GenericArguments[0].Name
                           };
            }

            if (!method.HasGenericParameters)
                return new BusMetadata()
                           {
                               Direction = Direction.Inbound,
                               MessageType = "OPAQUE RUNTIME SUBSCRIPTION"
                           };


            var gen2 = (GenericInstanceMethod) method;
            return new BusMetadata()
                       {
                           Direction = Direction.Inbound,
                           MessageType = gen2.GenericArguments[0].Name
                       };

        }

        static bool SkipAssembly(AssemblyDefinition assemblyDef)
        {
            if (assemblyDef.Name.Name.ToLower().StartsWith("masstransit"))
                return true;

            //no sense in analyzing system assemblies
            if (assemblyDef.Name.Name.ToLower().StartsWith("system"))
                return true;

            if (assemblyDef.Name.Name.ToLower().StartsWith("microsoft"))
                return true;

            return false;
        }
    }
}