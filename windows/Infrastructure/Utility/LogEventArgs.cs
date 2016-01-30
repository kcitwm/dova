using System;
using System.Collections.Generic;
using System.IO;
using System.Text; 

namespace Dova.Utility
{
    public class LogEventArgs: EventArgs
    {
        string msg = "";
        public string Msg
        {
            get { return msg; }
        }

        string action = "";
        public string Action
        {
            get { return action; }
        }

        public LogEventArgs(string action,string msg)
        {
            this.action = action;
            this.msg = msg;
        }

        public Dictionary<string, StringBuilder> logs;

        public Queue<KeyValuePair<string, string>> logQ; 

        public LogEventArgs(Dictionary<string, StringBuilder> logs)
        {
            this.logs = logs;
        }

        

        public LogEventArgs(Queue<KeyValuePair<string, string>> logQ)
        {
            this.logQ = logQ;
        }


    }
}
