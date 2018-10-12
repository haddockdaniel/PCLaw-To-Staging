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
    public class ExplCodeToStaging
    {

        public void insertIntoStaging(PLConvert.PCLawConversion PCLaw)
        {

            using (SqlConnection connection = new SqlConnection("Data Source=localhost;Initial Catalog=PCLawStg;Integrated Security=SSPI;"))
            {
                connection.Open();
                while (PCLaw.ExpCode.GetNextRecord() == 0)
                {
                    using (SqlCommand command1 = new SqlCommand())
                    {
                        command1.Connection = connection;
                        command1.CommandType = CommandType.Text;
                        command1.CommandText = "INSERT into ExplanationCode ([ExplanationCodeID], [OldID], [Name], [NickName], [ExplType], [GLAcctID], [RateAmount]) VALUES (@ExplanationCodeID, @OldID, @Name, @NickName, @ExplType, @GLAcctID, @RateAmount)";
                        command1.Parameters.AddWithValue("@ExplanationCodeID", PCLaw.ExpCode.ID);
                        command1.Parameters.AddWithValue("@OldID", PCLaw.ExpCode.ID);
                        command1.Parameters.AddWithValue("@NickName", PCLaw.ExpCode.NickName);
                        command1.Parameters.AddWithValue("@Name", PCLaw.ExpCode.Name);
                        command1.Parameters.AddWithValue("@ExplType", getExpCodeType(PCLaw.ExpCode.ExplType));
                        command1.Parameters.AddWithValue("@GLAcctID", PCLaw.ExpCode.GLNN);
                        command1.Parameters.Add("@RateAmount", SqlDbType.Decimal).Value = PCLaw.ExpCode.RateAmount;
                       // try
                      //  {

                            command1.ExecuteNonQuery();
                      //  }
                     //   catch (SqlException ex1)
                      //  {
                            //MessageBox.Show(ex1.Message);
                     //   }

                    }



                }
            }

        }

        private int getExpCodeType(PLExpCode.eEXPL_TYPE type)
        {
                switch (type.ToString().Trim())
                {
                    case "EXPENSE":
                        return 1;
                    case "TIME":
                        return 2;
                    default:
                        return 3;

                }
        }

    }
}
