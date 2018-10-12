using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace PCLaw_To_Staging
{
    public class UserToStaging
    {


        public void insertIntoStaging(PLConvert.PCLawConversion PCLaw)
        {

            using (SqlConnection connection = new SqlConnection("Data Source=localhost;Initial Catalog=PCLawStg;Integrated Security=SSPI;"))
            {
                connection.Open();
                while (PCLaw.User.GetNextRecord() == 0)
                {
                    if (PCLaw.User.Status == PLConvert.PLXMLData.eSTATUS.ACTIVE)
                    {
                        using (SqlCommand command1 = new SqlCommand())
                        {
                            command1.Connection = connection;
                            command1.CommandType = CommandType.Text;
                            command1.CommandText = "INSERT into [User] ([UserID], [nickname], [Name], [password]) VALUES (@UserID, @nickname, @Name, @Password)";
                            command1.Parameters.AddWithValue("@UserID", PCLaw.User.ID);
                            command1.Parameters.AddWithValue("@nickname", PCLaw.User.NickName);
                            command1.Parameters.AddWithValue("@Name", PCLaw.User.Name);
                            command1.Parameters.AddWithValue("@Password", "admin");
                            // try
                            // {

                            command1.ExecuteNonQuery();
                            //}
                            // catch (SqlException ex1)
                            // {
                            //     MessageBox.Show(ex1.Message);
                            //}

                        }
                    }


                }
            }

        }
    }
}
