using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCLaw_To_Staging
{
    public class WIPFee
    {
        public int ID { get; set; }
        public string matter { get; set; }
        public string lawyer { get; set; }
        public string date { get; set; }
        public double quantity { get; set; }
        public double duration { get; set; }
        public double rate { get; set; }
        public double totalPrice { get; set; }
        public string taskID { get; set; }
        public string explanation { get; set; }
        public string status { get; set; }
        public bool wip { get; set; }
    }
}
