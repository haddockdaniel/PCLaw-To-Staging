using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using PLConvert;

namespace PCLaw_To_Staging
{
    public class TaskCodeToStaging
    {

        public void insertIntoStaging(PLConvert.PCLawConversion PCLaw)
        {

            using (SqlConnection connection = new SqlConnection("Data Source=localhost;Initial Catalog=PCLawStg;Integrated Security=SSPI;"))
            {
                connection.Open();
                while (PCLaw.Task.GetNextRecord() == 0)
                {
                    using (SqlCommand command1 = new SqlCommand())
                    {
                        command1.Connection = connection;
                        command1.CommandType = CommandType.Text;
                        command1.CommandText = "INSERT into TaskCode ([TaskCodeID], [OldID], [NickName], [Name], [Category], [TypeOfLawID]) VALUES (@TaskCodeID, @OldID, @NickName, @Name, @Category, @TypeOfLawID)";
                        command1.Parameters.AddWithValue("@TaskCodeID", PCLaw.Task.ID);
                        command1.Parameters.AddWithValue("@OldID", PCLaw.Task.ID);
                        command1.Parameters.AddWithValue("@NickName", PCLaw.Task.NickName);
                        command1.Parameters.AddWithValue("@Name", PCLaw.Task.Name);
                        command1.Parameters.AddWithValue("@Category", getTaskType(PCLaw.Task.Category));
                        command1.Parameters.AddWithValue("@TypeOfLawID", PCLaw.Task.TypeOfLawNN);
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

        private int getTaskType(PLTask.eCATEGORY type)
        {
            switch (type.ToString().Trim())
            {
                case "BILLABLE":
                    return 1;
                case "NON_BILLABLE":
                    return 2;
                case "WRITE_UP_DOWN":
                    return 3;
                default:
                    return 1;
            }//end switch

        }
    }
}
