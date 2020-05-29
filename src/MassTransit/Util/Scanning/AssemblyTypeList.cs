namespace MassTransit.Util.Scanning
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;


    public class AssemblyTypeList
    {
        public readonly IList<Type> Abstract = new List<Type>();
        public readonly IList<Type> Concrete = new List<Type>();
        public readonly IList<Type> Interface = new List<Type>();

        public IEnumerable<IList<Type>> SelectTypes(TypeClassification classification)
        {
            var interfaces = classification.HasFlag(TypeClassification.Interface);
            var concretes = classification.HasFlag(TypeClassification.Concrete);
            var abstracts = classification.HasFlag(TypeClassification.Abstract);

            if (interfaces || concretes || abstracts)
            {
                if (interfaces)
                    yield return Interface;
                if (abstracts)
                    yield return Abstract;
                if (concretes)
                    yield return Concrete;
            }
            else
            {
                yield return Interface;
                yield return Abstract;
                yield return Concrete;
            }
        }

        public IEnumerable<Type> AllTypes()
        {
            return Interface.Concat(Concrete).Concat(Abstract);
        }

        public void Add(Type type)
        {
            if (type.GetTypeInfo().IsInterface)
                Interface.Add(type);
            else if (type.GetTypeInfo().IsAbstract)
                Abstract.Add(type);
            else if (type.GetTypeInfo().IsClass)
                Concrete.Add(type);
        }
    }
}
