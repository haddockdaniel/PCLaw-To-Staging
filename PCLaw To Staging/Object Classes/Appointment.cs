using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCLaw_To_Staging
{
    public class Appointment
    {
        public Appointment()
        {
            lawyerIDs = "";
            contactIDs = "";
        }

        public int AppointmentID { get; set; }
        public string enteredDate { get; set; }
        public string startDate { get; set; }
        public string dueDate { get; set; }
        public string OldID { get; set; }
        public string startTime { get; set; }
        public string description { get; set; }
        public bool isRecurring { get; set; }
        public string recurStartDate { get; set; }
        public string recurEndDate { get; set; }
        public int recurringType { get; set; }
        public int rDaysDaily { get; set; }
        public int rDayOfWeek { get; set; }
        public int rDayOfMonth { get; set; }
        public int rWeekOfMonth { get; set; }
        public int rMonthsQuarterly { get; set; }
        public int rMonthsOfYear { get; set; }
        public int holidayRecurBehav { get; set; }
        public int entryType { get; set; }
        public int priority { get; set; }
        public bool completed { get; set; }
        public string completionDate { get; set; }
        public string lawyerIDs { get; set; }
        public string contactIDs { get; set; }
        public string matterID { get; set; }
        public string reminderDate { get; set; }
        public int minutesToRemind { get; set; }

    }
}
