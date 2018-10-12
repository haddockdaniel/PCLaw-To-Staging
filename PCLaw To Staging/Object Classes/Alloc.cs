using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCLaw_To_Staging
{
    public class Alloc
    {
        public int ID { get; set; }
        public string oldID { get; set; }
        public string date { get; set; }
        public double total { get; set; }
        public double quantity { get; set; }
        public double rate { get; set; }
        public string lawyer { get; set; }
        public string glAcct { get; set; }
        public string billID { get; set; }
        public string code { get; set; }
        public int timeExp { get; set; }
        public string descr { get; set; }
    }
}
