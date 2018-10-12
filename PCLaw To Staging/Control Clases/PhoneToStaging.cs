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
    public class PhoneToStaging
    {
        List<PhoneCall> clientList = new List<PhoneCall>();

        public void insertIntoStaging(PLConvert.PCLawConversion PCLaw)
        {

            using (SqlConnection connection = new SqlConnection("Data Source=localhost;Initial Catalog=PCLawStg;Integrated Security=SSPI;"))
            {
                connection.Open();
                while (PCLaw.Diary.GetNextRecord() == 0)
                {
                    Diary d = new Diary();
                    if (PCLaw.Diary.EntryType == PLDiary.eType.Call)
                    using (SqlCommand command1 = new SqlCommand())
                    {
                        command1.Connection = connection;
                        command1.CommandType = CommandType.Text;
                        command1.CommandText = "INSERT into [PhoneCall] ([PhoneCallID] ,[OldID] ,[Duration], [OutGoingCall] ,[CallerName] ,[CallerNumber] ,[DueDate] ,[EnteredDate] ,[StartTime] ,[EndTime] ,[Description] ,[CallActionTaken] ,[PhoneCallReturned] ,[IsUrgent] ,[Completed] ,[CompletedDate] ,[MatterID],[UserID] ,[ContactID]) VALUES (@PhoneCallID ,@OldID, @Duration ,@OutGoingCall ,@CallerName ,@CallerNumber ,@DueDate ,@EnteredDate ,@StartTime ,@EndTime ,@Description ,@CallActionTaken ,@PhoneCallReturned ,@IsUrgent ,@Completed ,@CompletedDate ,@MatterID,@UserID ,@ContactID)";
                        command1.Parameters.AddWithValue("@PhoneCallID", PCLaw.Diary.ID);
                        command1.Parameters.AddWithValue("@EnteredDate", PCLaw.Diary.EnteredDate);
                        command1.Parameters.AddWithValue("@OutGoingCall", PCLaw.Diary.CallDirectionOut);
                        command1.Parameters.AddWithValue("@DueDate", PCLaw.Diary.DueDate);
                        command1.Parameters.AddWithValue("@StartTime", PCLaw.Diary.StartTime);
                        command1.Parameters.AddWithValue("@OldID", PCLaw.Diary.ID);
                        command1.Parameters.AddWithValue("@Description", PCLaw.Diary.Description);
                        command1.Parameters.AddWithValue("@PhoneCallReturned", PCLaw.Diary.PhoneCallReturnedCall);
                        command1.Parameters.AddWithValue("@CallActionTaken", getActionTaken(PCLaw.Diary.PhoneCallAction));
                        command1.Parameters.AddWithValue("@CallerName", PCLaw.Diary.CallContactName);
                        command1.Parameters.AddWithValue("@CallerNumber", PCLaw.Diary.CallPhoneNumber);
                        command1.Parameters.AddWithValue("@EndTime", 0);
                        command1.Parameters.AddWithValue("@IsUrgent", getPriority(PCLaw.Diary.Priority));
                        command1.Parameters.AddWithValue("@Completed", PCLaw.Diary.Completed);
                        command1.Parameters.AddWithValue("@CompletedDate", PCLaw.Diary.CompletionDate);
                        command1.Parameters.AddWithValue("@MatterID", PCLaw.Diary.MatterID);
                        command1.Parameters.AddWithValue("@UserID", 1);
                        command1.Parameters.AddWithValue("@ContactID", 1);
                        command1.Parameters.Add("@Duration", SqlDbType.Decimal).Value = PCLaw.Diary.Duration;
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

        private int getActionTaken(PLConvert.PLDiary.eCallAction action)
        {
            switch (action.ToString().Trim())
            {
                // 1 = spoke, 2 = Left Message, 3 = No Answer, 4 = Busy, 5 = Voice Mail
                case "Spoke":
                    return 1;
                case "LeftMsg":
                    return 2;
                case "NoAnswer":
                    return 3;
                case "Busy":
                    return 4;
                case "VoiceMail":
                    return 5;
                default:
                    return 3;
            }//end switch
        }

        private bool getPriority(PLConvert.PLDiary.ePriority priority)
        {
            switch (priority.ToString().Trim())
            {
                case "Normal":
                    return false;
                case "High":
                    return true;
                default:
                    return false;
            }//end switch

        }
    }
}
