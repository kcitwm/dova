
using Dova;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace System
{
    public static class Extensions
    {
        static Encoding gb2312 = Encoding.GetEncoding("GB2312");
        public static Encoding GB2312(this Encoding encoding)
        {
            return gb2312;
        }

        public static int ToInt32(this string input)
        {
            int n = 0;
            Int32.TryParse(input, out n);
            return n;
        }


        public static bool ToBoolean(this string input)
        {
            bool bl = false;
            bool.TryParse(input, out bl);
            return bl;
        }


        public static string ToKeyString(this DatabaseParameter[] input)
        {
            StringBuilder sb=new StringBuilder();
            sb.Append("Parameters:");
            foreach(DatabaseParameter  p in input)
            {
                sb.Append(p.ToKeyString());
            }
            return sb.ToString();
        }

        

        public static long ToInt64(this string input)
        {
            long n = 0;
            Int64.TryParse(input, out n);
            return n;
        }

        public static string TrimSql(this string input, int len)
        {
            return input.Substring(0, input.Length > len ? len : input.Length).Trim().Replace("\t", "  ");
        }


        public static float ToFloat(this string input)
        {
            float n = 0;
            float.TryParse(input, out n);
            return n;
        }

        public static decimal ToDecimal(this string input)
        {
            decimal n = 0;
            decimal.TryParse(input, out n);
            return n;
        }

        public static double ToDouble(this string input)
        {
            double n = 0;
            double.TryParse(input, out n);
            return n;
        }

        public static string ToKey(this object obj)
        { 
            return "" ;
        }

        public static DateTime ToDateTime(this string input)
        {
            DateTime dt;
            DateTime.TryParse(input, out dt);
            return dt;
        }

        public static void CloseConnection(this ICommunicationObject client)
        {
            if (client.State == CommunicationState.Faulted)
            {
                client.Abort();
                return;
            }
            try
            {
                client.Close();
            }
            catch (Exception e)
            {
                client.Abort();
            }
        }


        public static TKey GetKey<TKey, TValue>(this Dictionary<TKey, TValue> dic, int index)
        { 
            IEnumerator<TKey> e = dic.Keys.GetEnumerator();
            if (index < 0 || index > dic.Count)
                throw new IndexOutOfRangeException();
            int i = 0;
            while (e.MoveNext() && i != index)
            {
                i++;
            }
            return e.Current;

        }

        public static TValue GetValue<TKey, TValue>(this Dictionary<TKey, TValue> dic, int index)
        {
            IEnumerator<TValue> e = dic.Values.GetEnumerator();
            if (index < 0 || index > dic.Count)
                throw new IndexOutOfRangeException();
            int i = 0;
            while (e.MoveNext() && i != index)
            {
                i++;
            }
            return e.Current;

        }

        public static int IndexOfKey<TKey, TValue>(this Dictionary<TKey, TValue> dic, TKey k)
        { 
            IEnumerator<TKey> e = dic.Keys.GetEnumerator();
            int i = 0;
            while (e.MoveNext())
            {
                if (e.Current.Equals(k))
                    return i;
                i++;
            }
            return -1;
        }

        public static int IndexOfValue<TKey, TValue>(this Dictionary<TKey, TValue> dic, TValue v)
        {
            IEnumerator<TValue> e = dic.Values.GetEnumerator();
            int i = 0;
            while (e.MoveNext())
            {
                if (e.Current.Equals(v))
                    return i;
                i++;
            }
            return -1;
        }


        public static TKey GetCircleNextKey<TKey, TValue>(this Dictionary<TKey, TValue> dic, TKey current)
        {
            int count = dic.Count;
            if(1==dic.Count)
                return current;
            int idx = dic.IndexOfKey(current)+1; 
            if (idx == count) idx = 0;
            return dic.GetKey(idx);
        }



    }
}
