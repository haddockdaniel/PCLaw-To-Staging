using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCLaw_To_Staging
{
    public class PhoneCall
    {
        public int PhoneCallID { get; set; }
        public string enteredDate { get; set; }
        public string dueDate { get; set; }
        public string OldID { get; set; }
        public string startTime { get; set; }
        public string endTime { get; set; }
        public string description { get; set; }
        public bool isOutgoing { get; set; }
        public string callerName { get; set; }
        public string callerNumber { get; set; }
        public bool completed { get; set; }
        public bool returnedCall { get; set; }
        public string completionDate { get; set; }
        public bool isUrgent { get; set; }
        public string UserIDs { get; set; }
        public string contactIDs { get; set; }
        public string matterID { get; set; }
        public int callActionTaken { get; set; }
    }
}
