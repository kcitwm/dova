using System;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
using EmitMapper;

namespace Dova.Infrastructure
{
    public class EntityBuilder<T>    where T : new() 
    {
        private static readonly MethodInfo getValueMethod = typeof(DataRow).GetMethod("get_Item", new Type[] { typeof(int) });
        private static readonly MethodInfo isNullMethod = typeof(DataRow).GetMethod("IsNull", new Type[] { typeof(int) });
        private static readonly MethodInfo isDBNullMethod = typeof(IDataRecord).GetMethod("IsDBNull", new Type[] { typeof(int) });
        private static readonly MethodInfo getRecordValueMethod = typeof(IDataRecord).GetMethod("get_Item", new Type[] { typeof(int) });
        private delegate T LoadDataRow(DataRow dataRow);
        private LoadDataRow dataRowHandler;
        private delegate T LoadRecord(IDataRecord dataRecord);
        private LoadRecord recordHander;
        private delegate T LoadObject(object dataRow); 

        private static Dictionary<string, EntityBuilder<T>> builders = new Dictionary<string, EntityBuilder<T>>();
        static ObjectMapperManager omm = ObjectMapperManager.DefaultInstance;
         
        public T BuildEntity(DataRow dataRow)
        {
            return dataRowHandler(dataRow);
        }

        public T BuildEntity(IDataRecord dataRecord)
        {
            return recordHander(dataRecord);
        }

        private static EntityBuilder<T> CreateBuilder(DataRow dataRow)
        {
            Type type = typeof(T);
            string key = "Row:" + type.Name + ":" + dataRow.Table.TableName;
            if (builders.ContainsKey(key))
                return builders[key];
            EntityBuilder<T> dynamicBuilder = new EntityBuilder<T>();
            DynamicMethod method = new DynamicMethod("DataRowCreateT", type, new Type[] { typeof(DataRow) }, type, true);
            ILGenerator generator = method.GetILGenerator();
            LocalBuilder result = generator.DeclareLocal(type);
            generator.Emit(OpCodes.Newobj, type.GetConstructor(Type.EmptyTypes));
            generator.Emit(OpCodes.Stloc, result);
            int len = dataRow.ItemArray.Length;
            BindingFlags flag = BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.GetProperty;
            for (int i = 0; i < len; i++)
            {
                System.Reflection.PropertyInfo propertyInfo = type.GetProperty(dataRow.Table.Columns[i].ColumnName, flag);
                Label endIfLabel = generator.DefineLabel();
                if (propertyInfo != null && propertyInfo.GetSetMethod() != null)
                {
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldc_I4, i);
                    generator.Emit(OpCodes.Callvirt, isNullMethod);
                    generator.Emit(OpCodes.Brtrue, endIfLabel);
                    generator.Emit(OpCodes.Ldloc, result);
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldc_I4, i);
                    generator.Emit(OpCodes.Callvirt, getValueMethod);
                    generator.Emit(OpCodes.Unbox_Any, propertyInfo.PropertyType);
                    generator.Emit(OpCodes.Callvirt, propertyInfo.GetSetMethod());
                    generator.MarkLabel(endIfLabel);
                }
            }
            generator.Emit(OpCodes.Ldloc, result);
            generator.Emit(OpCodes.Ret);
            dynamicBuilder.dataRowHandler = (LoadDataRow)method.CreateDelegate(typeof(LoadDataRow));
            builders.Add(key, dynamicBuilder);
            return dynamicBuilder;
        }

        private static EntityBuilder<T> CreateBuilder(IDataRecord dataRecord)
        {
            Type type = typeof(T);
            string key = "Record:" + type.Name;
            if (builders.ContainsKey(key))
                return builders[key];
            EntityBuilder<T> dynamicBuilder = new EntityBuilder<T>();
            DynamicMethod method = new DynamicMethod("DataRecordCreateT", type, new Type[] { typeof(IDataRecord) }, type, true);
            ILGenerator generator = method.GetILGenerator();
            LocalBuilder result = generator.DeclareLocal(type);
            generator.Emit(OpCodes.Newobj, type.GetConstructor(Type.EmptyTypes));
            generator.Emit(OpCodes.Stloc, result);
            int len = dataRecord.FieldCount;
            BindingFlags flag=BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.GetProperty;
            for (int i = 0; i < len; i++)
            {
                PropertyInfo propertyInfo = type.GetProperty(dataRecord.GetName(i),flag );
                Label endIfLabel = generator.DefineLabel();
                if (propertyInfo != null && propertyInfo.GetSetMethod() != null)
                {
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldc_I4, i);
                    generator.Emit(OpCodes.Callvirt, isDBNullMethod);
                    generator.Emit(OpCodes.Brtrue, endIfLabel);
                    generator.Emit(OpCodes.Ldloc, result);
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldc_I4, i);
                    generator.Emit(OpCodes.Callvirt, getRecordValueMethod);
                    generator.Emit(OpCodes.Unbox_Any, propertyInfo.PropertyType);
                    // generator.Emit(OpCodes.Unbox_Any, dataRecord.GetFieldType(i));
                    generator.Emit(OpCodes.Callvirt, propertyInfo.GetSetMethod());
                    generator.MarkLabel(endIfLabel);
                }
            }
            generator.Emit(OpCodes.Ldloc, result);
            generator.Emit(OpCodes.Ret);
            dynamicBuilder.recordHander = (LoadRecord)method.CreateDelegate(typeof(LoadRecord));
            builders.Add(key, dynamicBuilder);
            return dynamicBuilder;
        }

 
        public  static T Builder<TSource>(TSource obj)
        {
            ObjectsMapper<TSource, T> map = omm.GetMapper<TSource, T>();
            return map.Map(obj);          
        }
         

        public static List<T> BuildList(DataTable dt)
        {
            List<T> list = new List<T>();
            if (dt == null || dt.Rows.Count==0) return list; 
            EntityBuilder<T> builder = EntityBuilder<T>.CreateBuilder(dt.Rows[0]);
            foreach (DataRow info in dt.Rows)
                list.Add(builder.BuildEntity(info));
            dt.Dispose(); dt = null;
            return list;
        }

        public static T Build(DataRow row)
        { 
            EntityBuilder<T> builder = CreateBuilder(row);
            return builder.BuildEntity(row);
        }

        public static T Build(DataTable dt)
        { 
            EntityBuilder<T> builder = CreateBuilder(dt.Rows[0]);
            return builder.BuildEntity(dt.Rows[0]);
        }

        public static List<T> BuildList(IDataReader dr)
        {
            EntityBuilder<T> builder = CreateBuilder(dr);
            List<T> list = new List<T>();
            if (dr == null) return list;
            while (dr.Read())
                list.Add(builder.BuildEntity(dr));
            return list;
        }

        public static T Build(IDataReader dr)
        {
            EntityBuilder<T> builder = CreateBuilder(dr);
            if (dr.Read())
                return builder.BuildEntity(dr); 
            return new T();  
        }

    }
}
