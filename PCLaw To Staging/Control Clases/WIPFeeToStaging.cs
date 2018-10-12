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
    public class WIPFeeToStaging
    {

        List<WIPFee> wipFeeList = new List<WIPFee>();

        public void getTimeFrimPCLaw14(PCLawConversion PCLaw)
        {
            int count = 1;
            using (SqlConnection connection = new SqlConnection("Data Source=localhost;Initial Catalog=PCLawStg;Integrated Security=SSPI;"))
            {
                connection.Open();

                //add the gl accounts minus the bank accounts
                while (PCLaw.TimeEntry.GetNextRecord() == 0)
                {
                    
                    if (PCLaw.TimeEntry.InvDate == 0 && PCLaw.TimeEntry.InvoiceID == 0) // it IS NOT in the list of bank accounts so we add it. Bank accounts are automatically added as GL accounts when added above
                    {
                        using (SqlCommand command1 = new SqlCommand())
                        {
                            command1.Connection = connection;
                            command1.CommandType = CommandType.Text;
                            command1.CommandText = "INSERT into WIPFee ([WIPFeeID] ,[EntryDate] ,[MatterID] ,[LawyerID] ,[Duration] ,[Rate] ,[TotalBillable] ,[TaskCodeID],[Status],[Explanation]) VALUES (@WIPFeeID ,@EntryDate ,@MatterID ,@LawyerID ,@Duration ,@Rate ,@TotalBillable ,@TaskCodeID,@Status,@Explanation)";
                            command1.Parameters.AddWithValue("@WIPFeeID", count);
                            command1.Parameters.AddWithValue("@EntryDate", PCLaw.TimeEntry.DateEntered);
                            command1.Parameters.AddWithValue("@MatterID", PCLaw.TimeEntry.MatterID);
                            command1.Parameters.AddWithValue("@LawyerID", PCLaw.TimeEntry.LawyerID);
                            command1.Parameters.AddWithValue("@TaskCodeID", PCLaw.TimeEntry.TaskID);
                            command1.Parameters.AddWithValue("@Status", PCLaw.TimeEntry.Status);
                            command1.Parameters.AddWithValue("@Explanation", PCLaw.TimeEntry.Explanation);
                            command1.Parameters.Add("@TotalBillable", SqlDbType.Decimal).Value = PCLaw.TimeEntry.Amount;
                            command1.Parameters.Add("@Rate", SqlDbType.Decimal).Value = PCLaw.TimeEntry.Rate;
                            command1.Parameters.Add("@Duration", SqlDbType.Decimal).Value = PCLaw.TimeEntry.Hours;
                            try
                            {

                                command1.ExecuteNonQuery();
                            }
                            catch (SqlException ex1)
                            {
                                MessageBox.Show(ex1.Message);
                            }

                        }
                        count++;
                    }

                }



            }
            MessageBox.Show(count.ToString());
        }

        public void createWIPFeeList(List<string> lawyerList)
        {


            WIPFee wipFee;
            string lawyerForSQL = "";
            foreach (string lawyer in lawyerList)
                lawyerForSQL = lawyerForSQL + lawyer + ",";

            lawyerForSQL = lawyerForSQL.TrimEnd(',');

            string sql = @"SELECT * FROM [TimeEnt] where TimeEntryStatus = 0 and matterid in (SELECT  [MatterID] FROM [MattInf] where matterinfostatus = 0 and MatterInfoRespLwyr in (" + lawyerForSQL + ")) ";
            SqlConnection con = new SqlConnection("Data Source=localhost;Initial Catalog=PCLAWDB_80290;Integrated Security=SSPI;");
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

                            wipFee = new WIPFee();
                            wipFee.ID = count;
                            wipFee.date = reader["TimeEntryDate"].ToString();
                            wipFee.matter = reader["MatterID"].ToString().Trim();
                            wipFee.lawyer = reader["LawyerID"].ToString().Trim();
                            wipFee.taskID = reader["TimeEntryActivity"].ToString().Trim();
                            wipFee.explanation = reader["TimeEntryExplanation"].ToString().Trim();
                            wipFee.status = "1";
                            if (double.Parse(reader["TimeEntryActualHours"].ToString().Trim()) > 500000.00) //this happens when the amount is negative. PCLaw stores it as an int (seconds) and if it is negative, it makes it this giant number so we do the math and take the absolute value of that (cant have negative time)
                                wipFee.duration = Math.Abs(double.Parse(reader["TimeEntryAmount"].ToString().Trim()) / double.Parse(reader["TimeEntryActualRate"].ToString().Trim()));
                            else
                                wipFee.duration = double.Parse(reader["TimeEntryActualHours"].ToString().Trim());
                            wipFee.rate = double.Parse(reader["TimeEntryActualRate"].ToString().Trim());
                            wipFee.totalPrice = double.Parse(reader["TimeEntryAmount"].ToString().Trim());
                            //these values will be 0 for qip and not for AR allocations
                            if (reader["TimeEntryInvID"].ToString().Trim().Equals("0") && reader["TimeEntryInvDate"].ToString().Trim().Equals("0"))
                                wipFee.wip = true;
                            else
                                wipFee.wip = false;
                            wipFeeList.Add(wipFee);
                            count++;

                        }
                        catch (Exception inner1)
                        { MessageBox.Show("Inner: " + inner1.Message); }
                    }
                }
            }

        }



        public void insertIntoStaging(PLConvert.PCLawConversion PCLaw)
        {

            using (SqlConnection connection = new SqlConnection("Data Source=localhost;Initial Catalog=PCLawStg;Integrated Security=SSPI;"))
            {
                connection.Open();
                int alloc = 1;
                foreach (WIPFee wip in wipFeeList)
                {
                    if (wip.wip) //it is not assigned an invoice so we add it as wip
                    {
                            using (SqlCommand command1 = new SqlCommand())
                            {
                                command1.Connection = connection;
                                command1.CommandType = CommandType.Text;
                                command1.CommandText = "INSERT into [WIPFee] ([WIPFeeID],[EntryDate],[MatterID],[LawyerID],[Duration],[Rate],[TotalBillable] ,[TaskCodeID],[Status],[Explanation], [ExplanationCodeID] ) VALUES (@WIPFeeID,@EntryDate,@MatterID,@LawyerID,@Duration,@Rate,@TotalBillable ,@TaskCodeID,@Status,@Explanation, @ExplanationCodeID)";
                                command1.Parameters.AddWithValue("@WIPFeeID", wip.ID);
                                command1.Parameters.AddWithValue("@EntryDate", wip.date);
                                command1.Parameters.AddWithValue("@MatterID", wip.matter);
                                command1.Parameters.AddWithValue("@LawyerID", wip.lawyer);
                                command1.Parameters.AddWithValue("@TaskCodeID", wip.taskID);
                                command1.Parameters.AddWithValue("@Status", wip.status);
                                command1.Parameters.AddWithValue("@Explanation", wip.explanation);
                                command1.Parameters.Add("@TotalBillable", SqlDbType.Decimal).Value = wip.totalPrice;
                                command1.Parameters.Add("@Rate", SqlDbType.Decimal).Value = wip.rate;
                                command1.Parameters.Add("@Duration", SqlDbType.Decimal).Value = wip.duration;
                                try
                                {

                                    command1.ExecuteNonQuery();
                                }
                                catch (SqlException ex1)
                                {
                                    MessageBox.Show("Fee: " + ex1.Message);
                                }

                            }
                        }
                        else  //it IS assigned to AR so we add it as an allocation
                        {
                            using (SqlCommand command1 = new SqlCommand())
                            {
                                command1.Connection = connection;
                                command1.CommandType = CommandType.Text;
                                command1.CommandText = "INSERT into [Allocation] ([AllocationID] ,[OldID], [MatterID], [TimeStatus] ,[Total] ,[QuantityHours],[Rate] ,[LawyerID] ,[GLAcctID] ,[EntryDate] ,[BillID] ,[CodeID] ,[TimeExp] ,[Description] ,[ExpensePaidTo]) VALUES (@AllocationID ,@OldID, @MatterID, @TimeStatus ,@Total ,@QuantityHours,@Rate ,@LawyerID ,@GLAcctID ,@EntryDate ,@BillID ,@CodeID ,@TimeExp ,@Description ,@ExpensePaidTo)";
                                command1.Parameters.AddWithValue("@AllocationID", alloc);
                                command1.Parameters.AddWithValue("@OldID", PCLaw.TimeEntry.ID);
                                command1.Parameters.AddWithValue("@LawyerID", PCLaw.TimeEntry.LawyerID);
                                command1.Parameters.AddWithValue("@MatterID", PCLaw.TimeEntry.MatterID);
                                //command1.Parameters.AddWithValue("@TimeStatus", getStatus(PCLaw.TimeEntry.HoldFlag));
                                command1.Parameters.AddWithValue("@GLAcctID", 0);
                                command1.Parameters.AddWithValue("@EntryDate", PCLaw.TimeEntry.Date);
                                command1.Parameters.AddWithValue("@BillID", PCLaw.TimeEntry.InvoiceID);
                                command1.Parameters.AddWithValue("@CodeID", PCLaw.TimeEntry.TaskID);
                                command1.Parameters.AddWithValue("@TimeExp", 0);
                                command1.Parameters.AddWithValue("@Description", PCLaw.TimeEntry.Explanation);
                                command1.Parameters.AddWithValue("@ExpensePaidTo", "");
                                command1.Parameters.Add("@Total", SqlDbType.Decimal).Value = PCLaw.TimeEntry.Amount;
                                command1.Parameters.Add("@QuantityHours", SqlDbType.Decimal).Value = PCLaw.TimeEntry.Hours;
                                command1.Parameters.Add("@Rate", SqlDbType.Decimal).Value = PCLaw.TimeEntry.Rate;
                                try
                                {
                                    command1.ExecuteNonQuery();
                                }
                                catch (SqlException ex1)
                                {
                                    MessageBox.Show(ex1.Message);
                                }
                            }
                            alloc++;
                            
                        }
                    
                }
            }

        }

    }
}
