using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text; 

namespace Dova.Data
{
    public class Scripter
    {
        public string Name = string.Empty;
        public string CmdText = string.Empty;
        public int CmdType = (int)CommandType.Text;
        public string CacheKey = string.Empty;
        public string CacheParas = string.Empty;
        public TimeSpan CacheSpan = new TimeSpan(1, 0, 0);

        public Scripter(string name, string cmdText, int cmdType, string cacheKey, string cacheParas,string timeSpan)
        {
            this.Name = name;
            this.CmdText = cmdText;
            if(timeSpan!=string.Empty)
                this.CacheSpan=TimeSpan.Parse(timeSpan);
            this.CmdType = cmdType;
            this.CacheKey = cacheKey;
            this.CacheParas = cacheParas;
        }

        public Scripter(string name, string cmdText, int cmdType, string cacheKey, string cacheParas)
        {
            this.Name = name;
            this.CmdText = cmdText; 
            this.CmdType = cmdType;
            this.CacheKey = cacheKey;
            this.CacheParas = cacheParas;
        }

        string realCacheKey = string.Empty;
        public string GetCachekey(DbParameter[] pars)
        {
            if (realCacheKey == string.Empty)
            {
                realCacheKey = CacheKey;
                if (!string.IsNullOrEmpty(CacheParas) && pars.Length>0)
                {
                    string cp=CacheParas+",";
                    string values = string.Empty;
                    foreach (DbParameter p in pars)
                    {
                        if (cp.IndexOf(p.ParameterName + ",") > -1)
                        {
                            values += p.Value.ToString();
                        }
                    }
                    realCacheKey += ":" + CacheParas + ":" + values;
                }
            }
            return realCacheKey;
        }

    }
}
