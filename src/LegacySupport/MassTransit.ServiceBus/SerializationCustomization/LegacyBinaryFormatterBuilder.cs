namespace MassTransit.LegacySupport.SerializationCustomization
{
    using System.Collections.Generic;
    using System.Runtime.Serialization.Formatters.Binary;
    using Subscriptions;

    public class LegacyBinaryFormatterBuilder
    {
        //weak to strong
        public static BinaryFormatter BuildReader()
        {
            var newReader = new BinaryFormatter();

            var readerSelector = new LegacySurrogateSelector();
            readerSelector.AddSurrogate(new WeakToStrongListSurrogate<List<Subscription>, Subscription>("mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", "System.Collections.Generic.List`1[[MassTransit.ServiceBus.Subscriptions.Subscription, MassTransit.ServiceBus, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]"));


            newReader.SurrogateSelector = readerSelector;


            //smelly
            var b = new WeakToStrongBinder();
            var maps = new TypeMappings();

            foreach (TypeMap map in maps)
            {
                b.AddMap(map);
            }

            newReader.Binder = b;
            return newReader;
        }

        //strong to weak
        public static BinaryFormatter BuildWriter()
        {

            var newWriter = new BinaryFormatter();
            var ss = new LegacySurrogateSelector();

            var maps = new TypeMappings();

            foreach (TypeMap map in maps)
            {
                ss.AddSurrogate(new StrongToWeakItemSurrogate(map));
            }
            newWriter.SurrogateSelector = ss;

            return newWriter;
        }
    }
}