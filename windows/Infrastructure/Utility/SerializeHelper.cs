namespace Dova.Utility
{
    using System;
    using System.IO;
    using System.Text;
    using System.Xml.Serialization;
    using System.Runtime.Serialization.Json;
    using System.Runtime.Serialization.Formatters.Soap;
    using System.Runtime.Serialization.Formatters.Binary;

    using Dova.Utility;
    using Newtonsoft.Json;

    public class SerializeHelper
    {

        static Encoding encode = Config.Encode;

        public static T DeSerialize<T>(string format, object o)
        {
            if (format == "json")
                return DeSerializeToJSon<T>(o as string); 
            else
                return DeSerializeFromBytes<T>(o as byte[]);

        }

        public static object DeSerialize(string format, object o)
        { 
            if (format == "json")
                return o as string;
            return DeSerializeFromBytes(o as byte[]);

        }
         


        public static object Serialize(string format, object o)
        {
            if (o is string) return o;
            if (format == "json")
                return SerializeToJSon(o); 
            else
                return SerializeToBytes(o);

        }

        //public static object Serialize<T>(string format, T o)
        //{
        //    if (format == "json")
        //        return SerializeToJSon(o);
        //    else
        //        return SerializeToBytes(o);

        //}


        public static T DeSerializeFromBinary<T>(string fileName)
        {
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                using (FileStream stream = new FileStream(fileName, FileMode.Open))
                {
                    return (T)formatter.Deserialize(stream);
                }
            }
            catch (Exception exception)
            {
                Log.Error(exception);
            }
            return default(T);
        }

        public static object DeSerializeFromBinary(string fileName)
        {
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                using (FileStream stream = new FileStream(fileName, FileMode.Open))
                {
                    return formatter.Deserialize(stream);
                }
            }
            catch (Exception exception)
            {
                Log.Error(exception);
            }
            return null;
        }

        public static object DeSerializeFromBytes(byte[] buffer)
        {
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                using (MemoryStream stream = new MemoryStream())
                {
                    stream.Write(buffer, 0, buffer.Length);
                    stream.Position = 0L;
                    return formatter.Deserialize(stream);
                }
            }
            catch (Exception exception)
            {
                Log.Error(exception);
            }
            return null;
        }

        public static object DeSerializeFromBytes(string format,byte[] buffer)
        {
            if (format == "json")
                return encode.GetString(buffer);
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                using (MemoryStream stream = new MemoryStream())
                {
                    stream.Write(buffer, 0, buffer.Length);
                    stream.Position = 0L;
                    return formatter.Deserialize(stream);
                }
            }
            catch (Exception exception)
            {
                Log.Error(exception);
            }
            return null;
        }


        public static T DeSerializeFromBytes<T>(byte[] buffer)
        {
            return (T)DeSerializeFromBytes(buffer);
        }

        public static T DeSerializeFromSoapXml<T>(string XmlFile)
        {
            try
            {
                SoapFormatter formatter = new SoapFormatter();
                using (FileStream stream = new FileStream(XmlFile, FileMode.Open))
                {
                    return (T)formatter.Deserialize(stream);
                }
            }
            catch (Exception exception)
            {
                Log.Error(exception);
            }
            return default(T);
        }

        public static T DeSerializeFromXml<T>(string xmlFile)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                using (FileStream stream = new FileStream(xmlFile, FileMode.Open))
                {
                    return (T)serializer.Deserialize(stream);
                }
            }
            catch (Exception exception)
            {
                Log.Error(exception);
            }
            return default(T);
        }


        public static T DeSerializeFromXmlDoc<T>(string xmlDoc)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                using (TextReader tr = new StringReader(xmlDoc))
                {
                    return (T)serializer.Deserialize(tr);
                }
            }
            catch (Exception exception)
            {
                Log.Error(exception);
            }
            return default(T);
        }


        public static void SerializeToBinary(object instance, string fileName)
        {
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                using (FileStream stream = new FileStream(fileName, FileMode.Create))
                {
                    formatter.Serialize(stream, instance);
                }
            }
            catch (Exception exception)
            {
                Log.Error(exception);
            }
        }

        public static byte[] SerializeToBytes(object instance)
        {
            if (instance == null)
            {
                return null;
            }
            byte[] buffer2 = null;
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                using (MemoryStream stream = new MemoryStream())
                {
                    formatter.Serialize(stream, instance);
                    stream.Position = 0L;
                    byte[] buffer = new byte[stream.Length];
                    stream.Read(buffer, 0, buffer.Length);
                    buffer2 = buffer;
                }
            }
            catch (Exception exception)
            {
                Log.Error(exception);
            }
            return buffer2;
        }


        public static byte[] SerializeToBytes(string format,object instance)
        {
            if (format == "json")
                return encode.GetBytes(SerializeToJSon(instance));
            byte[] buffer2 = null;
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                using (MemoryStream stream = new MemoryStream())
                {
                    formatter.Serialize(stream, instance);
                    stream.Position = 0L;
                    byte[] buffer = new byte[stream.Length];
                    stream.Read(buffer, 0, buffer.Length);
                    buffer2 = buffer;
                }
            }
            catch (Exception exception)
            {
                Log.Error(exception);
            }
            return buffer2;
        }

        public static void SerializeToSoapXml(object instance, string XmlFile)
        {
            try
            {
                SoapFormatter formatter = new SoapFormatter();
                using (FileStream stream = new FileStream(XmlFile, FileMode.Create))
                {
                    formatter.Serialize(stream, instance);
                }
            }
            catch (Exception exception)
            {
                Log.Error(exception);
            }
        }

        public static void SerializeToXml<T>(object instance, string xmlFile)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("", "");
                using (FileStream stream = new FileStream(xmlFile, FileMode.Create))
                {
                    serializer.Serialize((Stream)stream, instance,ns);
                }
            }
            catch (Exception exception)
            {
                Log.Error(exception);
            }
        }

        public static string SerializeToJSonContract<T>(object instance)
        {
            try
            {
                var serializer = new DataContractJsonSerializer(instance.GetType());
                using (var ms = new MemoryStream())
                {
                    serializer.WriteObject(ms, instance);
                    var sb = new StringBuilder();
                    sb.Append(Encoding.UTF8.GetString(ms.ToArray()));
                    return sb.ToString();
                }
            }
            catch (Exception e)
            {
                Log.Error("SerializeToJSonContract:" + e.Message + e.StackTrace);
            }
            return "";
        }


        public static string SerializeToJSon(object instance)
        {
            try
            {
                return JsonConvert.SerializeObject(instance);
            }
            catch (Exception e)
            {
                Log.Error("SerializeToJSon:" + e.Message + e.StackTrace);
            }
            return "";
        }


        public static string SerializeToJSon(object instance,bool ingnoreDefaultValue)
        {
            try
            {
                if (ingnoreDefaultValue)
                {
                    JsonSerializerSettings jss = new JsonSerializerSettings();
                    jss.DefaultValueHandling=DefaultValueHandling.Ignore;
                    return JsonConvert.SerializeObject(instance, jss);
                }
                return JsonConvert.SerializeObject(instance);
            }
            catch (Exception e)
            {
                Log.Error("SerializeToJSon:" + e.Message + e.StackTrace);
            }
            return "";
        }

        public static T DeSerializeToJSon<T>(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception e)
            {
                Log.Write("DeSerializeToJSon:" + e.Message + e.StackTrace);
            }
            return default(T);
        }

        public static object DeSerializeToJSon(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject(json);
            }
            catch (Exception e)
            {
                Log.Write("DeSerializeToJSon:" + e.Message + e.StackTrace);
            }
            return null;
        }



        public static T DeSerializeToJSonContract<T>(string json)
        {
            try
            {
                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                {
                    var jsonSerializer = new DataContractJsonSerializer(typeof(T));
                    T obj = (T)jsonSerializer.ReadObject(ms);
                    return obj;
                }
            }
            catch (Exception e)
            {
                Log.Write("DeSerializeToJSon:" + e.Message + e.StackTrace);
            }
            return default(T);
        }




    }
}

