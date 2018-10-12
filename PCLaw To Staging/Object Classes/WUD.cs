using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCLaw_To_Staging
{
    public class WUD
    {
        public int WUDID { get; set; }
        public string date { get; set; }
        public string BillID { get; set; }
        public string MatterID { get; set; }
        public double Amount { get; set; }
        public string Explanation { get; set; }
    }
}
