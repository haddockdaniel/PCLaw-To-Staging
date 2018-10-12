using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Data.SqlTypes;
using PLConvert;

namespace PCLaw_To_Staging
{
    public class ApptToStaging
    {

        public void insertIntoStaging(PLConvert.PCLawConversion PCLaw)
        {

            using (SqlConnection connection = new SqlConnection("Data Source=localhost;Initial Catalog=PCLawStg;Integrated Security=SSPI;"))
            {
                connection.Open();
                while (PCLaw.Diary.GetNextRecord() == 0)
                {
                    Diary d = new Diary();
                    if (PCLaw.Diary.EntryType == PLDiary.eType.Holiday || PCLaw.Diary.EntryType == PLDiary.eType.Appointment || PCLaw.Diary.EntryType == PLDiary.eType.TODO)
                    using (SqlCommand command1 = new SqlCommand())
                    {
                        command1.Connection = connection;
                        command1.CommandType = CommandType.Text;
                        command1.CommandText = "INSERT into [Appointment] ([AppointmentID],[EnteredDate],[DueDate], [Duration] ,[StartTime],[OldID],[Description],[IsRecurring],[RecurringStartDate] ,[RecurringEndDate], [RecurringType],[rDaysDaily],[rDayOfMonth],[rWeekOfMonth],[rDayOfWeek],[rMonthsQuarterly] ,[rMonthsOfYear],[HolidayRecurringBehavior] ,[EntryType],[Priority],[Completed],[CompletionDate],[MatterID],[LawyerID],[ContactID],[ReminderDate],[MinutesToRemind]) VALUES (@AppointmentID,@EnteredDate,@DueDate, @Duration ,@StartTime,@OldID,@Description,@IsRecurring,@RecurringStartDate ,@RecurringEndDate,@RecurringType,@rDaysDaily,@rDayOfMonth,@rWeekOfMonth,@rDayOfWeek,@rMonthsQuarterly ,@rMonthsOfYear, @HolidayRecurringBehavior ,@EntryType,@Priority,@Completed,@CompletionDate,@MatterID,@LawyerID,@ContactID,@ReminderDate,@MinutesToRemind)";
                        d = getRecurringType(PCLaw);
                        command1.Parameters.AddWithValue("@AppointmentID", PCLaw.Diary.ID);
                        command1.Parameters.AddWithValue("@EnteredDate", PCLaw.Diary.EnteredDate);
                        command1.Parameters.AddWithValue("@Duration", PCLaw.Diary.Duration);
                        command1.Parameters.AddWithValue("@DueDate", PCLaw.Diary.DueDate);
                        command1.Parameters.AddWithValue("@StartTime", "");
                        command1.Parameters.AddWithValue("@OldID", PCLaw.Diary.ID);
                        command1.Parameters.AddWithValue("@Description", PCLaw.Diary.Description);
                        command1.Parameters.AddWithValue("@IsRecurring", PCLaw.Diary.IsRecurringEntry);
                        command1.Parameters.AddWithValue("@RecurringStartDate", PCLaw.Diary.RecurringStartDate);
                        command1.Parameters.AddWithValue("@RecurringEndDate", PCLaw.Diary.RecurringEndDate);
                        command1.Parameters.AddWithValue("@RecurringType",d.recurringType);
                        command1.Parameters.AddWithValue("@rDaysDaily", d.daysDaily);
                        command1.Parameters.AddWithValue("@rDayOfMonth", d.dayOfMonth);
                        command1.Parameters.AddWithValue("@rWeekOfMonth", d.weekOfMonth);
                        command1.Parameters.AddWithValue("@rDayOfWeek", d.dayOfWeek);
                        command1.Parameters.AddWithValue("@rMonthsQuarterly", d.monthsQuarterly);
                        command1.Parameters.AddWithValue("@rMonthsOfYear", d.monthOfYear);
                        command1.Parameters.AddWithValue("@HolidayRecurringBehavior", d.holiday);
                        command1.Parameters.AddWithValue("@EntryType", getEntryType(PCLaw));
                        command1.Parameters.AddWithValue("@Priority", PCLaw.Diary.Priority);
                        command1.Parameters.AddWithValue("@Completed", PCLaw.Diary.Completed);
                        command1.Parameters.AddWithValue("@CompletionDate", PCLaw.Diary.CompletionDate);
                        command1.Parameters.AddWithValue("@MatterID", PCLaw.Diary.MatterID);
                        command1.Parameters.AddWithValue("@LawyerID", 1);
                        command1.Parameters.AddWithValue("@ContactID", 1);
                        command1.Parameters.AddWithValue("@ReminderDate", PCLaw.Diary.ReminderDate);
                        command1.Parameters.AddWithValue("@MinutesToRemind", PCLaw.Diary.MinutesToRemind);
                        try
                        {

                            command1.ExecuteNonQuery();
                        }
                        catch (SqlException ex1)
                        {
                            MessageBox.Show(ex1.Message);
                        }

                    }

                }
            }

        }


        private int getEntryType(PLConvert.PCLawConversion PCLaw)
        {
                switch (PCLaw.Diary.EntryType.ToString().Trim())
                {
                    case "Holiday":
                        return 0;
                    case "Appointment":
                        return 1;
                    case "TODO":
                        return 2;
                    default:
                        return 1;
                }//end switch
        }

        private Diary getRecurringType(PLConvert.PCLawConversion PCLaw)
        {
            Diary d = new Diary();
            switch (PCLaw.Diary.RecurringRepeatUnit.ToString().Trim())
            {
                case "CalendarDay":   //Daily
                    d.recurringType = 1;
                    d.daysDaily = PCLaw.Diary.RecurringFreq;
                    d.dayOfWeek = 0;
                    d.holiday = 0;
                    d.monthOfYear = 0;
                    d.weekOfMonth = 0;
                    d.dayOfMonth = 0;
                    d.monthsQuarterly = 0;
                    break;

                case "Month": //Monthly   
                    d.recurringType = 2;
                    d.daysDaily = PCLaw.Diary.RecurringFreq;
                    d.dayOfWeek = 0;
                    d.holiday = 0;
                    d.monthOfYear = 0;
                    d.monthsQuarterly = 0;
                    if (PCLaw.Diary.RecurringDayOfMonth != 0)
                        d.dayOfMonth = PCLaw.Diary.RecurringDayOfMonth;
                    else
                    {
                        if (PCLaw.Diary.RecurringWeekOfMonth != 0)
                            d.weekOfMonth = PCLaw.Diary.RecurringWeekOfMonth;
                        else
                            d.weekOfMonth = 1;
                        d.dayOfWeek = getDayOfWeek(PCLaw);

                    }//end else
                    d.daysDaily = PCLaw.Diary.RecurringFreq;
                    break;

                case "Week": //Weekly
                    d.recurringType = 4;
                    d.holiday = 0;
                    d.monthOfYear = 0;
                    d.monthsQuarterly = 0;
                    d.dayOfMonth = 0;
                    d.weekOfMonth = 0;
                    d.dayOfWeek = getDayOfWeek(PCLaw);
                    d.daysDaily = PCLaw.Diary.RecurringFreq;
                    break;
                case "Year": //Annually
                    d.recurringType = 5;
                    d.holiday = 0;
                    d.monthOfYear = 0;
                    d.monthsQuarterly = 0;
                    d.dayOfMonth = 0;
                    d.weekOfMonth = 0;
                    d.monthOfYear = PCLaw.Diary.RecurringMonthOfYear;

                    if (PCLaw.Diary.RecurringDayOfMonth != 0)
                        d.dayOfMonth = PCLaw.Diary.RecurringDayOfMonth;
                    else
                    {
                        d.weekOfMonth = PCLaw.Diary.RecurringWeekOfMonth;
                        d.dayOfWeek = getDayOfWeek(PCLaw);
                    }//end else

                    d.daysDaily = PCLaw.Diary.RecurringFreq;
                    break;
            }//end switch with no default on purpose

            switch (PCLaw.Diary.RecurringAdjust.ToString().Trim())
            {
                case "NextBusDay":
                    d.holiday = 1;
                    break;
                case "PrevBusDay":
                    d.holiday = 2;
                    break;
                case "SameDay":
                    d.holiday = 3;
                    break;
                case "CancelEntry":
                    d.holiday = 4;
                    break;
            }//end switch
            return d;
        }

        private int getDayOfWeek(PLConvert.PCLawConversion PCLaw)
        {
            switch (PCLaw.Diary.RecurringDayOfWeek.ToString().Trim())
            {
                case "Sun":
                    return 1;
                case "Non":
                    return 2;
                case "Tue":
                    return 3;
                case "Wed":
                    return 4;
                case "Thur":
                    return 5;
                case "Fri":
                    return 6;
                case "Sat":
                    return 7;
                default:
                    return 2;
            }//end switch
        }
    }
}
