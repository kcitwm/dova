using System;
using EmitMapper;
using System.Collections.Generic;
using EmitMapper.MappingConfiguration;
using EmitMapper.MappingConfiguration.MappingOperations;

namespace Dova.Infrastructure
{
    public class ObjectTransfer<TFrom, To>
    {
        static ObjectsMapper<TFrom, To> map = ObjectMapperManager.DefaultInstance.GetMapper<TFrom, To>();
    //    ObjectsMapper<TFrom, To> mapper1 =
    //new ObjectMapperManager().GetMapper<TFrom, To>(
    //    new DefaultMapConfig()
    //    .ConstructBy<TFrom>(new TargetConstructor<TFrom>(() => default(TFrom))));

        public static To Transfer(TFrom from)
        { 
            return map.Map(from);
        }

        public static To Transfer(TFrom from, To to)
        {
            return map.Map(from, to);
        }


        public static List<To> Transfer(List<TFrom> froms)
        {
            if (null == froms) return null;
            List<To> lists = new List<To>();
            foreach (TFrom from in froms) 
                lists.Add(map.Map(from));
            return lists;
        }

        public static List<To> Transfer(IList<TFrom> froms)
        {
            if (null == froms) return null;
            List<To> lists =new List<To>();
            foreach (TFrom from in froms)
                lists.Add(map.Map(from));
            return lists;
        }



        public static List<To> Transfer(List<TFrom> froms, List<To> tos)
        {
            List<To> lists = new List<To>();
            foreach (TFrom from in froms)
                foreach(To to in tos)
                    lists.Add(map.Map(from,to));
            return lists;
        } 
    }

}
