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
    public class ToLToStaging
    {

        public void insertIntoStaging(PLConvert.PCLawConversion PCLaw)
        {

            using (SqlConnection connection = new SqlConnection("Data Source=localhost;Initial Catalog=PCLawStg;Integrated Security=SSPI;"))
            {
                connection.Open();
                while (PCLaw.TypeOfLaw.GetNextRecord() == 0)
                {
                    using (SqlCommand command1 = new SqlCommand())
                    {
                        command1.Connection = connection;
                        command1.CommandType = CommandType.Text;
                        command1.CommandText = "INSERT into TypeOfLaw ([TypeOfLawID], [oldid], [nickname], [Name]) VALUES (@TypeOfLawID, @oldid, @nickname, @Name)";
                        command1.Parameters.AddWithValue("@TypeOfLawID", PCLaw.TypeOfLaw.ID);
                        command1.Parameters.AddWithValue("@oldid", PCLaw.TypeOfLaw.ID);
                        command1.Parameters.AddWithValue("@nickname", PCLaw.TypeOfLaw.NickName);
                        command1.Parameters.AddWithValue("@Name", PCLaw.TypeOfLaw.Name);
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
