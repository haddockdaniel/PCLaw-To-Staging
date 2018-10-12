using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCLaw_To_Staging
{
    public class Bill
    {
        public int ID { get; set; }
        public string oldID { get; set; }
        public string matter { get; set; }
        public string date { get; set; }
        public double disb { get; set; }
        public double fees { get; set; }
        public double taxes { get; set; }
        public double hours { get; set; }
        public string respLawyer { get; set; }
        public string typeOfLaw { get; set; }
        public string billNo { get; set; }
        public double interest { get; set; }
    }
}
