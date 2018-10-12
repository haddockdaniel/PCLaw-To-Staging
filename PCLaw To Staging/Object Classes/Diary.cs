using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCLaw_To_Staging
{
    public class Diary
    {
        public int recurringType { get; set; }
        public int weekOfMonth { get; set; }
        public int dayOfWeek { get; set; }
        public int monthOfYear { get; set; }
        public int holiday { get; set; }
        public int daysDaily { get; set; }
        public int dayOfMonth { get; set; }
        public int monthsQuarterly { get; set; }
    }
}
