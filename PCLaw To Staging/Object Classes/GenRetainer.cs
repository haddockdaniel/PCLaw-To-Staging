using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCLaw_To_Staging
{
    public class GenRetainer
    {
        public string invoiceID { get; set; }
        public string bankAccount { get; set; }
        public string date { get; set; }
        public double payment { get; set; }
        public string explanation { get; set; }
        public string matterID { get; set; }
        public string checkNo { get; set; }
        public int ID { get; set; }
    }
}
