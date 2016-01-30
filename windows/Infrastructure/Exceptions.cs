using Dova.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dova.Infrastructure
{
    public class Exceptions
    { 
        /// <summary>
        /// 异常处理
        /// </summary>
        /// <param name="e">异常</param>
        /// <param name="isThrow">是否抛出异常</param>
        public static void DoExeption(Exception e,bool isThrow)
        {
            string msg = e.ToString();
            if (null != e.InnerException)
                msg += e.ToString();
            Log.Error(msg);
            if (isThrow) throw e;
        }

        /// <summary>
        /// 异常处理
        /// </summary>
        /// <param name="e">异常</param>
        /// <param name="isThrow">是否抛出异常</param>
        /// <param name="preMsg">添加在异常消息前面的文字</param>
        public static void DoExeption(Exception e, bool isThrow,string preMsg)
        {
            string msg =preMsg+":" + e.ToString();
            if (null != e.InnerException)
                msg += e.ToString();
            Log.Error(msg);
            if (isThrow) throw e;
        } 

    }
}
