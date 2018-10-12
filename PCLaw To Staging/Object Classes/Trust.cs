using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCLaw_To_Staging
{
    public class Trust
    {
        public int ID { get; set; }
        public double amount { get; set; }
        public int entryType { get; set; }
        public int paymentType { get; set; }
        public string tbAcct { get; set; }
        public string paidTo { get; set; }
        public string date { get; set; }
        public string checkNo { get; set; }
        public string matter { get; set; }
        public string explanation { get; set; }
    }
}
