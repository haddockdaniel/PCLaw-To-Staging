using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCLaw_To_Staging
{
    public class Note
    {
        public int NoteID { get; set; }
        public string enteredDate { get; set; }
        public string reminderDate { get; set; }
        public string dueDate { get; set; }
        public string OldID { get; set; }
        public string description { get; set; }
        public string shortDesc { get; set; }
        public string LawyerIDs { get; set; }
        public string contactIDs { get; set; }
        public string matterID { get; set; }
    }
}
