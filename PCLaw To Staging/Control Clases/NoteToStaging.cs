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
    public class NoteToStaging
    {
        //
        List<Note> clientList = new List<Note>();

        public void insertIntoStaging(PLConvert.PCLawConversion PCLaw)
        {
                while (PCLaw.Diary.GetNextRecord() == 0)
                {
                    Diary d = new Diary();
                    if (PCLaw.Diary.EntryType == PLDiary.eType.Notes)
            using (SqlConnection connection = new SqlConnection("Data Source=localhost;Initial Catalog=PCLawStg;Integrated Security=SSPI;"))
            {
                connection.Open();

                    using (SqlCommand command1 = new SqlCommand())
                    {
                        command1.Connection = connection;
                        command1.CommandType = CommandType.Text;
                        command1.CommandText = "INSERT into [Note] ([NoteID] ,[OldID] ,[DueDate] ,[EnteredDate] , [ReminderDate], [ShortDescription],[Description],[MatterID],[LawyerIDs] ,[ContactIDs], [ContactIDsAreClientIDs]) VALUES (@NoteID ,@OldID ,@DueDate ,@EnteredDate , @ReminderDate, @ShortDescription,@Description,@MatterID,@LawyerIDs ,@ContactIDs, @ContactIDsAreClientIDs)";
                        command1.Parameters.AddWithValue("@NoteID", PCLaw.Diary.ID);
                        command1.Parameters.AddWithValue("@EnteredDate", PCLaw.Diary.EnteredDate);
                        command1.Parameters.AddWithValue("@ReminderDate", PCLaw.Diary.ReminderDate);
                        command1.Parameters.AddWithValue("@DueDate", PCLaw.Diary.DueDate);
                        command1.Parameters.AddWithValue("@ShortDescription", PCLaw.Diary.NoteShortDescription);
                        command1.Parameters.AddWithValue("@OldID", PCLaw.Diary.ID);
                        command1.Parameters.AddWithValue("@Description", PCLaw.Diary.Description);
                        command1.Parameters.AddWithValue("@LawyerIDs", 1);
                        command1.Parameters.AddWithValue("@MatterID", PCLaw.Diary.MatterID);
                        command1.Parameters.AddWithValue("@ContactIDs", 1);
                        command1.Parameters.AddWithValue("@ContactIDsAreClientIDs", 1);
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
    }
}
