using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Data.SqlTypes;

namespace PCLaw_To_Staging
{
    public class GenRetainerToStaging
    {
        List<GenRetainer> clientList = new List<GenRetainer>();

        public void createPaymentList()
        {
            GenRetainer genRet;
            string sql = @"select * from [LedgerHistory] inner join  Matter on MatSysNbr = LHMatter and MatStatusFlag <> 'C' where LHType in ('6', '5')";
            SqlConnection con = new SqlConnection("Data Source=localhost;Initial Catalog=Juris5573000;Integrated Security=SSPI;");
            using (var command = new SqlCommand(sql, con))
            {
                con.Open();
                int count = 1;
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        try
                        {

                            genRet = new GenRetainer();
                            genRet.ID = count;
                            string[] dates = reader["lhDate"].ToString().Trim().Split(' ');
                            string[] final = dates[0].Split('/');
                            if (final[0].Length < 2)
                                final[0] = "0" + final[0];
                            if (final[1].Length < 2)
                                final[1] = "0" + final[1];
                            genRet.date = final[2] + final[0] + final[1];
                            genRet.checkNo = count.ToString();
                            genRet.invoiceID = "";
                            genRet.bankAccount = "CZB";
                            genRet.explanation = reader["lhcomment"].ToString().Trim();
                            genRet.matterID = reader["lhmatter"].ToString().Trim();
                            genRet.payment = double.Parse(reader["lhcashamt"].ToString().Trim());
                            if (reader["lhtype"].ToString().Trim().Equals("6"))
                                genRet.payment = genRet.payment * -1;
                            clientList.Add(genRet);
                            count++;

                        }
                        catch (Exception inner1)
                        { MessageBox.Show("Inner: " + inner1.Message); }
                    }
                }
            }

        }

        public void insertIntoStaging()
        {

            using (SqlConnection connection = new SqlConnection("Data Source=localhost;Initial Catalog=PCLawStg;Integrated Security=SSPI;"))
            {
                connection.Open();

                foreach (GenRetainer c in clientList)
                {
                    using (SqlCommand command1 = new SqlCommand())
                    {

                        command1.Connection = connection;
                        command1.CommandType = CommandType.Text;
                        command1.CommandText = "INSERT into [GenRetainer] ([GenRetainerID] ,[InvoiceID],[CheckNo],[BankAcct] ,[Date],[Payment],[Explanation],[MatterID]) VALUES (@GenRetainerID ,@InvoiceID,@CheckNo,@BankAcct ,@Date,@Payment,@Explanation,@MatterID)";
                        command1.Parameters.AddWithValue("@GenRetainerID", c.ID);
                        command1.Parameters.AddWithValue("@Date", c.date);
                        command1.Parameters.AddWithValue("@MatterID", c.matterID);
                        command1.Parameters.AddWithValue("@InvoiceID", c.invoiceID);
                        command1.Parameters.AddWithValue("@BankAcct", c.bankAccount);
                        command1.Parameters.AddWithValue("@CheckNo", c.checkNo);
                        command1.Parameters.AddWithValue("@Explanation", c.explanation);
                        command1.Parameters.Add("@Payment", SqlDbType.Decimal).Value = c.payment;
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
