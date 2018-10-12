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
    public class WIPExpToStaging
    {

        public void insertIntoStaging(PLConvert.PCLawConversion PCLaw, List<Matter> mats)
        {
            List<Matter> matterList = mats.ToList();
            //here we have to get all the expense entries without an invoice as well as the checks without an invoice
            using (SqlConnection connection = new SqlConnection("Data Source=localhost;Initial Catalog=PCLawStg;Integrated Security=SSPI;"))
            {
                connection.Open();
                //gets the last id added to the allocations table so we can add the expenses to it
                int alloc = 0;
                string sql = "SELECT MAX(allocationid) FROM Allocation";

                SqlCommand cmd = new SqlCommand(sql, connection);
                SqlDataReader rd = cmd.ExecuteReader();
                if (rd.HasRows)
                {
                    rd.Read(); // read first row
                    try
                    {
                        alloc = rd.GetInt32(0);
                    }
                    catch (Exception e)
                    {
                        alloc = 1;
                    }
                }
                rd.Close();
                cmd.Dispose();
                //end get last alloc id

                alloc++;

                //get all expense entries and either add them as wip (no invoice) or ar allocations (has invoice)
                while (PCLaw.Expense.GetNextRecord() == 0)
                {
                   // matterList = null;
                    int matterIndex = matterList.FindIndex(f => f.ID == PCLaw.Expense.MatterID);
                    if (matterIndex > -1 && PCLaw.Expense.Status == PLXMLData.eSTATUS.ACTIVE) // it IS in the list of selected matters
                    {
                        if (PCLaw.Expense.InvoiceNumber < 1) //if it is not assigned an invoiceid (WIP)
                        {
                            using (SqlCommand command1 = new SqlCommand())
                            {
                                command1.Connection = connection;
                                command1.CommandType = CommandType.Text;
                                command1.CommandText = "INSERT into [WIPExpense] ([WIPExpenseID],[EntryDate],[MatterID],[Quantity],[Rate],[TotalExpensed] ,[ExplanationCodeID], [GLAcctID],[Explanation], [ExpensePaidTo]) VALUES (@WIPExpenseID,@EntryDate,@MatterID,@Quantity,@Rate,@TotalExpensed ,@ExplanationCodeID, @GLAcctID,@Explanation, @ExpensePaidTo)";
                                command1.Parameters.AddWithValue("@WIPExpenseID", PCLaw.Expense.ID);
                                command1.Parameters.AddWithValue("@EntryDate", PCLaw.Expense.Date);
                                command1.Parameters.AddWithValue("@MatterID", PCLaw.Expense.MatterID);
                                command1.Parameters.AddWithValue("@GLAcctID", PCLaw.Expense.GLID);
                                command1.Parameters.AddWithValue("@ExplanationCodeID", PCLaw.Expense.ExplCodeID);
                                command1.Parameters.AddWithValue("@Explanation", PCLaw.Expense.Explanation);
                                command1.Parameters.AddWithValue("@ExpensePaidTo", PCLaw.Expense.PaidTo);
                                command1.Parameters.Add("@TotalExpensed", SqlDbType.Decimal).Value = PCLaw.Expense.Amount;
                                command1.Parameters.Add("@Rate", SqlDbType.Decimal).Value = PCLaw.Expense.Rate;
                                command1.Parameters.Add("@Quantity", SqlDbType.Decimal).Value = PCLaw.Expense.Quantity;
                                try
                                {
                                    command1.ExecuteNonQuery();
                                }
                                catch (SqlException ex1)
                                {
                                    MessageBox.Show("Exp: " + ex1.Message);
                                }

                            }
                        }
                        else //if it has an invoiceid it is AR, not wip so add it as an allocation
                        {
                            using (SqlCommand command2 = new SqlCommand())
                            {
                                command2.Connection = connection;
                                command2.CommandType = CommandType.Text;
                                command2.CommandText = "INSERT into [Allocation] ([AllocationID] ,[OldID], [MatterID], [TimeStatus] ,[Total] ,[QuantityHours],[Rate] ,[LawyerID] ,[GLAcctID] ,[EntryDate] ,[BillID] ,[CodeID] ,[TimeExp] ,[Description] ,[ExpensePaidTo]) VALUES (@AllocationID ,@OldID, @MatterID, @TimeStatus ,@Total ,@QuantityHours,@Rate ,@LawyerID ,@GLAcctID ,@EntryDate ,@BillID ,@CodeID ,@TimeExp ,@Description ,@ExpensePaidTo)";
                                command2.Parameters.AddWithValue("@AllocationID", alloc);
                                command2.Parameters.AddWithValue("@OldID", PCLaw.Expense.ID);
                                command2.Parameters.AddWithValue("@LawyerID", 0);
                                command2.Parameters.AddWithValue("@MatterID", PCLaw.Expense.MatterID);
                                command2.Parameters.AddWithValue("@TimeStatus", 0);
                                command2.Parameters.AddWithValue("@GLAcctID", PCLaw.Expense.GLID);
                                command2.Parameters.AddWithValue("@EntryDate", PCLaw.Expense.Date);
                                command2.Parameters.AddWithValue("@BillID", PCLaw.Expense.InvoiceID);
                                command2.Parameters.AddWithValue("@CodeID", PCLaw.Expense.ExplCodeID);
                                command2.Parameters.AddWithValue("@TimeExp", 1);
                                command2.Parameters.AddWithValue("@Description", PCLaw.Expense.Explanation);
                                command2.Parameters.AddWithValue("@ExpensePaidTo", PCLaw.Expense.PaidTo);
                                command2.Parameters.Add("@Total", SqlDbType.Decimal).Value = PCLaw.Expense.Amount;
                                command2.Parameters.Add("@QuantityHours", SqlDbType.Decimal).Value = PCLaw.Expense.Quantity;
                                command2.Parameters.Add("@Rate", SqlDbType.Decimal).Value = PCLaw.Expense.Rate;
                                try
                                {
                                    command2.ExecuteNonQuery();
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
                
                //get all general allocations, make sure they are checks, have a matter number (we only assign expenses assigned to matters) and see if they have an
                //invoice number. If not, wip, if they do, ar allocation
                //if check and no invoiceid (must have matters - not 0)
                while (PCLaw.General.GetNextRecord() == 0)
                {
     
                    List<PLGBAlloc> allocation = PCLaw.General.GetAllocArray().ToList();
                    foreach (PLGBAlloc a in allocation)
                    {
                        //matterList = null;
                        int matterIndex = matterList.FindIndex(f => f.ID == a.MatterID);
                        if (matterIndex > -1 && PCLaw.General.PmtMethodFlag == PLGBEnt.ePmtMethod.Check && PCLaw.General.Status == PLXMLData.eSTATUS.ACTIVE &&
                            (a.EntryType == PLGBAlloc.eGBAllocEntryType.EXP_COB || a.EntryType == PLGBAlloc.eGBAllocEntryType.EXP_REC)) // it IS in the list of selected matters and is a check
                        {
                            if (a.InvID < 1) //if it is not assigned an invoiceid (WIP)
                            {
                                using (SqlCommand command1 = new SqlCommand())
                                {
                                    command1.Connection = connection;
                                    command1.CommandType = CommandType.Text;
                                    command1.CommandText = "INSERT into [WIPExpense] ([WIPExpenseID],[EntryDate],[MatterID],[Quantity],[Rate],[TotalExpensed] ,[ExplanationCodeID], [GLAcctID],[Explanation], [ExpensePaidTo]) VALUES (@WIPExpenseID,@EntryDate,@MatterID,@Quantity,@Rate,@TotalExpensed ,@ExplanationCodeID, @GLAcctID,@Explanation, @ExpensePaidTo)";
                                    command1.Parameters.AddWithValue("@WIPExpenseID", a.ID);
                                    command1.Parameters.AddWithValue("@EntryDate", PCLaw.General.Date);
                                    command1.Parameters.AddWithValue("@MatterID", a.MatterID);
                                    command1.Parameters.AddWithValue("@GLAcctID", a.GLID);
                                    command1.Parameters.AddWithValue("@ExplanationCodeID", a.ExplCodeID);
                                    command1.Parameters.AddWithValue("@Explanation", a.Explanation);
                                    command1.Parameters.AddWithValue("@ExpensePaidTo", PCLaw.General.PaidTo);
                                    command1.Parameters.Add("@TotalExpensed", SqlDbType.Decimal).Value = a.Amount;
                                    command1.Parameters.Add("@Rate", SqlDbType.Decimal).Value = a.Amount;
                                    command1.Parameters.Add("@Quantity", SqlDbType.Decimal).Value = 1;
                                    try
                                    {
                                        command1.ExecuteNonQuery();
                                    }
                                    catch (SqlException ex1)
                                    {
                                        MessageBox.Show("Exp: " + ex1.Message);
                                    }

                                }
                            }
                            else //if it has an invoiceid it is AR, not wip so add it as an allocation
                            {
                                using (SqlCommand command2 = new SqlCommand())
                                {
                                    command2.Connection = connection;
                                    command2.CommandType = CommandType.Text;
                                    command2.CommandText = "INSERT into [Allocation] ([AllocationID] ,[OldID], [MatterID], [TimeStatus] ,[Total] ,[QuantityHours],[Rate] ,[LawyerID] ,[GLAcctID] ,[EntryDate] ,[BillID] ,[CodeID] ,[TimeExp] ,[Description] ,[ExpensePaidTo]) VALUES (@AllocationID ,@OldID, @MatterID, @TimeStatus ,@Total ,@QuantityHours,@Rate ,@LawyerID ,@GLAcctID ,@EntryDate ,@BillID ,@CodeID ,@TimeExp ,@Description ,@ExpensePaidTo)";
                                    command2.Parameters.AddWithValue("@AllocationID", alloc);
                                    command2.Parameters.AddWithValue("@OldID", PCLaw.General.ID);
                                    command2.Parameters.AddWithValue("@LawyerID", 0);
                                    command2.Parameters.AddWithValue("@MatterID", a.MatterID);
                                    command2.Parameters.AddWithValue("@TimeStatus", 0);
                                    command2.Parameters.AddWithValue("@GLAcctID", a.GLID);
                                    command2.Parameters.AddWithValue("@EntryDate", PCLaw.General.Date);
                                    command2.Parameters.AddWithValue("@BillID", a.InvID);
                                    command2.Parameters.AddWithValue("@CodeID", a.ExplCodeID);
                                    command2.Parameters.AddWithValue("@TimeExp", 1);
                                    command2.Parameters.AddWithValue("@Description", a.Explanation);
                                    command2.Parameters.AddWithValue("@ExpensePaidTo", PCLaw.General.PaidTo);
                                    command2.Parameters.Add("@Total", SqlDbType.Decimal).Value = a.Amount;
                                    command2.Parameters.Add("@QuantityHours", SqlDbType.Decimal).Value = 1;
                                    command2.Parameters.Add("@Rate", SqlDbType.Decimal).Value = a.Amount;
                                    try
                                    {
                                        command2.ExecuteNonQuery();
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
    }
}
