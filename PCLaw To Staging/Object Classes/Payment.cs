using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCLaw_To_Staging
{
    public class Payment
    {
        public int ID { get; set; }
        public string oldID { get; set; }
        public string date { get; set; }
        public double received { get; set; }
        public double applied { get; set; }
        public string checkNo { get; set; }
        public string transID { get; set; }
        public string bankAcctID { get; set; }
        public string client { get; set; }
        public string glAcct { get; set; }
        public int paymentType { get; set; }
        public string matter { get; set; }
        public int paymentClass { get; set; }
        public string invoice { get; set; }
        public string explanation { get; set; }
    }
}
