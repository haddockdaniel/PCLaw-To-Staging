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
    public class WUDtoStaging
    {
        List<WUD> clientList = new List<WUD>();

        public void createPaymentList()
        {
            WUD client;


            string sql = @"select LHBillNbr, LHMatter, MAX(LHDate) AS LHDate, SUM(LHFees) AS LHFees, SUM(LHCshExp) AS LHCshExp, SUM(LHNCshExp) AS LHNCshExp, SUM(LHTaxes1) AS LHTaxes1, SUM(LHTaxes2) AS LHTaxes2, SUM(LHTaxes3) AS LHTaxes3, SUM(LHInterest) AS LHInterest, SUM(LHSurcharge) AS LHSurcharge, 'Write up/down' AS LHComment from [LedgerHistory] inner join  ARMatAlloc on LHMatter = ARMMatter and LHBillNbr = ARMBillNbr  inner join Matter on MatSysNbr = LHMatter and MatStatusFlag <> 'C' where LHType = '8'  GROUP BY LHBillNbr, LHMatter having SUM(LHFees) + SUM(LHCshExp)+ SUM(LHNCshExp)+SUM(LHTaxes1)+SUM(LHTaxes2)+SUM(LHTaxes3)+SUM(LHInterest)+SUM(LHSurcharge) < 0";
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
                            double amount = double.Parse(reader["LHTaxes1"].ToString().Trim()) + double.Parse(reader["LHTaxes2"].ToString().Trim()) + double.Parse(reader["LHTaxes3"].ToString().Trim()) + double.Parse(reader["LHSurcharge"].ToString().Trim()) + double.Parse(reader["LHInterest"].ToString().Trim()) + double.Parse(reader["LHFees"].ToString().Trim()) + double.Parse(reader["LHNCshExp"].ToString().Trim()) + double.Parse(reader["LHCshExp"].ToString().Trim());
                            if (amount <= 0) //only do the wuds that are negative (write downs)
                            {
                                client = new WUD();
                                client.WUDID = count;
                                string[] dates = reader["lhDate"].ToString().Trim().Split(' ');
                                string[] final = dates[0].Split('/');
                                if (final[0].Length < 2)
                                    final[0] = "0" + final[0];
                                if (final[1].Length < 2)
                                    final[1] = "0" + final[1];
                                client.date = final[2] + final[0] + final[1];
                                client.BillID = reader["lhbillnbr"].ToString().Trim();
                                client.Explanation = reader["lhcomment"].ToString().Trim();
                                client.MatterID = reader["lhmatter"].ToString().Trim();
                                client.Amount = amount;
                                clientList.Add(client);
                                count++;
                            }

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

                foreach (WUD c in clientList)
                {
                    using (SqlCommand command1 = new SqlCommand())
                    {

                        command1.Connection = connection;
                        command1.CommandType = CommandType.Text;
                        command1.CommandText = "INSERT into [WUD] ([WUDID],[Date],[BillID],[MatterID],[Amount],[Explanation]) VALUES (@WUDID,@Date,@BillID,@MatterID,@Amount,@Explanation)";
                        command1.Parameters.AddWithValue("@WUDID", c.WUDID);
                        command1.Parameters.AddWithValue("@Date", c.date);
                        command1.Parameters.AddWithValue("@BillID", c.BillID);
                        command1.Parameters.AddWithValue("@MatterID", c.MatterID);
                        command1.Parameters.AddWithValue("@Explanation", c.Explanation);
                        command1.Parameters.Add("@Amount", SqlDbType.Decimal).Value = c.Amount;
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
