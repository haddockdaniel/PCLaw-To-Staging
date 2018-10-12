using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCLaw_To_Staging
{
    public class WIPExp
    {
        public int ID { get; set; }
        public string matter { get; set; }
        public string date { get; set; }
        public double quantity { get; set; }
        public double totalExpense { get; set; }
        public double rate { get; set; }
        public double totalPrice { get; set; }
        public string explanationCodeID { get; set; }
        public string explanation { get; set; }
        public string glAcctID { get; set; }
    }
}
