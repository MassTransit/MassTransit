namespace MassTransit.Util.Scanning
{
    using System;
    using System.Collections.Generic;
    using System.Linq;


    public class AssemblyTypeList
    {
        public readonly List<Type> Abstract = new List<Type>();
        public readonly List<Type> Concrete = new List<Type>();
        public readonly List<Type> Interface = new List<Type>();

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
            if (type.IsInterface)
                Interface.Add(type);
            else if (type.IsAbstract)
                Abstract.Add(type);
            else if (type.IsClass)
                Concrete.Add(type);
        }
    }
}
