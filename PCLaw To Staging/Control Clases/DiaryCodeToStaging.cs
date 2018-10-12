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
    public class DiaryCodeToStaging
    {

        public void insertIntoStaging(PLConvert.PCLawConversion PCLaw)
        {

            using (SqlConnection connection = new SqlConnection("Data Source=localhost;Initial Catalog=PCLawStg;Integrated Security=SSPI;"))
            {
                connection.Open();
                while (PCLaw.DiaryCode.GetNextRecord() == 0)
                {
                    using (SqlCommand command1 = new SqlCommand())
                    {
                        command1.Connection = connection;
                        command1.CommandType = CommandType.Text;
                        command1.CommandText = "INSERT into DiaryCodes ([AppointmentCodeID], [OldID], [Name], [Nickname], [CodeType]) VALUES (@AppointmentCodeID, @OldID, @Name, @Nickname, @CodeType)";
                        command1.Parameters.AddWithValue("@AppointmentCodeID", PCLaw.DiaryCode.ID);
                        command1.Parameters.AddWithValue("@OldID", PCLaw.DiaryCode.ID);
                        command1.Parameters.AddWithValue("@Name", PCLaw.DiaryCode.Name);
                        command1.Parameters.AddWithValue("@Nickname", PCLaw.DiaryCode.NickName);
                        command1.Parameters.Add("@CodeType", SqlDbType.Decimal).Value = 1;
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
