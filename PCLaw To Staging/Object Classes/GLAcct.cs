using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCLaw_To_Staging
{
    public class GLAcct
    {
        public int GLAcctID { get; set; }
        public string OldID { get; set; }
        public string Nickname { get; set; }
        public string GLName { get; set; }
        public string BankAcctNumber { get; set; }
        public int Type { get; set; }
        public string BankType { get; set; }
    }
}
