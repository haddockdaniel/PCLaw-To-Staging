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
    public class AllocToStaging
    {
        List<Alloc> clientList = new List<Alloc>();
        /*
        public void createAllocList()
        {
            string sqlExpense = @"SELECT [BEBillNbr], [BEMatter] ,[BEExpCd] ,[BEDate],[BEUnitsOnBill] ,[BEAmtOnBill] ,[BENarrative], [glacct] FROM [BilledExpenses] where BEMatter in (select matsysnbr from matter where MatStatusFlag <> 'C')";
            
            SqlConnection con = new SqlConnection("Data Source=localhost;Initial Catalog=Juris5573000;Integrated Security=SSPI;");
            int count = 1;
            //time entries
            using (var command = new SqlCommand(sqlTimeFee, con))
            {
                con.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        //  try
                        //  {
                        client = new Alloc();
                        client.ID = count;
                        string[] dates = reader["BTDateOnBill"].ToString().Trim().Split(' ');
                        string[] final = dates[0].Split('/');
                        if (final[0].Length < 2)
                            final[0] = "0" + final[0];
                        if (final[1].Length < 2)
                            final[1] = "0" + final[1];
                        client.date = final[2] + final[0] + final[1];
                        client.code = "";
                        client.quantity = double.Parse(reader["BTHrsOnBill"].ToString().Trim());
                        client.rate = double.Parse(reader["BTRateOnBill"].ToString().Trim());
                        client.timeExp = 0;
                        client.total = double.Parse(reader["BTAmtOnBill"].ToString().Trim());
                        client.billID = reader["BTBillNbr"].ToString().Trim();
                        client.descr = reader["BTNarrative"].ToString().Trim();
                        client.lawyer = reader["BTTkpr"].ToString().Trim();
                        client.oldID = reader["BTBillNbr"].ToString().Trim();
                        client.glAcct = "";
                        clientList.Add(client);
                        count++;
                        //  }
                        //   catch (Exception inner1)
                        //  {
                        //     MessageBox.Show("Inner: " + inner1.Message);
                        // }
                    }
                }
            }
            con.Close();
            //expense entries
            using (var command = new SqlCommand(sqlExpense, con))
            {
                con.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        try
                        {
                            client = new Alloc();
                            client.ID = count;
                            string[] dates = reader["BEDate"].ToString().Trim().Split(' ');
                            string[] final = dates[0].Split('/');
                            if (final[0].Length < 2)
                                final[0] = "0" + final[0];
                            if (final[1].Length < 2)
                                final[1] = "0" + final[1];
                            client.date = final[2] + final[0] + final[1];
                            client.code = reader["BEExpCd"].ToString().Trim();
                            client.quantity = 1.00;
                            client.rate = double.Parse(reader["BEAmtOnBill"].ToString().Trim());
                            client.timeExp = 1;
                            client.total = double.Parse(reader["BEAmtOnBill"].ToString().Trim());
                            client.billID = reader["BEBillNbr"].ToString().Trim();
                            client.descr = reader["BENarrative"].ToString().Trim();
                            client.lawyer = "";
                            client.oldID = reader["BEBillNbr"].ToString().Trim();
                            client.glAcct = reader["glacct"].ToString().Trim();
                            clientList.Add(client);
                            count++;
                        }
                        catch (Exception inner1)
                        {
                            MessageBox.Show("Inner: " + inner1.Message);
                        }
                    }
                }
            }


        }
        */
        public void insertIntoStaging()
        {

            using (SqlConnection connection = new SqlConnection("Data Source=localhost;Initial Catalog=PCLawStg;Integrated Security=SSPI;"))
            {
                connection.Open();

                foreach (Alloc c in clientList)
                {
                    using (SqlCommand command1 = new SqlCommand())
                    {
                        command1.Connection = connection;
                        command1.CommandType = CommandType.Text;
                        command1.CommandText = "INSERT into [Allocation] ([AllocationID],[OldID],[Total],[QuantityHours],[Rate],[LawyerID],[GLAcctID],[EntryDate],[BillID],[CodeID],[TimeExp],[Description]) VALUES (@AllocationID,@OldID,@Total,@QuantityHours,@Rate,@LawyerID,@GLAcctID,@EntryDate,@BillID,@CodeID,@TimeExp,@Description)";
                        command1.Parameters.AddWithValue("@AllocationID", c.ID);
                        command1.Parameters.AddWithValue("@EntryDate", c.date);
                        command1.Parameters.AddWithValue("@GLAcctID", c.glAcct);
                        command1.Parameters.AddWithValue("@OldID", c.oldID);
                        command1.Parameters.AddWithValue("@BillID", c.billID);
                        command1.Parameters.AddWithValue("@LawyerID", c.lawyer);
                        command1.Parameters.AddWithValue("@TimeExp", c.timeExp);
                        command1.Parameters.AddWithValue("@CodeID", c.code);
                        command1.Parameters.AddWithValue("@Description", c.descr);
                        command1.Parameters.Add("@Total", SqlDbType.Decimal).Value = c.total;
                        command1.Parameters.Add("@QuantityHours", SqlDbType.Decimal).Value = c.quantity;
                        command1.Parameters.Add("@Rate", SqlDbType.Decimal).Value = c.rate;
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
