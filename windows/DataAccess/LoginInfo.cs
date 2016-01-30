using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 

namespace Dova.Data
{
    [Serializable]
    public class LoginRes
    {
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string MachineKey { get; set; }
        public string Location { get; set; } 
        public List<string> Roles { get; set; }
        public int Status { get; set; }
        public List<string> Rights { get; set; }
        public long UserType { get; set; }
    }
     
    [Serializable]
    public class LoginReq
    {
        public string AppName { get; set; }
        public string UserID { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public string MachineKey { get; set; }
        public string Location { get; set; }
        public string Email { get; set; }
        public long UserType { get; set; }
    }
     
}
