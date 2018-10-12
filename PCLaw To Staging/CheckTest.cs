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
    public class CheckTest
    {

        public void insertIntoStaging(PLConvert.PCLawConversion PCLaw, List<int> matterList)
        {
            //here we have to get all the expense entries without an invoice as well as the checks wothout an invoice
            using (SqlConnection connection = new SqlConnection("Data Source=localhost;Initial Catalog=PCLawStg;Integrated Security=SSPI;"))
            {
                connection.Open();


                //get all general allocations, make sure they are checks, have a matter number (we only assign expenses assigned to matters) and see if they have an
                //invoice number. If not, wip, if they do, ar allocation
                //if check and no invoiceid (must have matters - not 0)
                while (PCLaw.General.GetNextRecord() == 0)
                {
                    List<PLGBAlloc> allocation = PCLaw.General.GetAllocArray().ToList();
                    foreach (PLGBAlloc a in allocation)
                    {
                        int matterIndex = matterList.FindIndex(f => f == a.MatterID);
                        if (matterIndex > -1 && PCLaw.General.PmtMethodFlag == PLGBEnt.ePmtMethod.Check) // it IS in the list of selected matters and is a check
                        {
                            if (a.InvID < 1) //if it is not assigned an invoiceid (WIP)
                            {
                                    using (SqlCommand command1 = new SqlCommand())
                                    {
                                        command1.Connection = connection;
                                        command1.CommandType = CommandType.Text;
                                        command1.CommandText = "INSERT into [WIPExpense] ([WIPExpenseID],[EntryDate],[MatterID],[Quantity],[Rate],[TotalExpensed] ,[ExplanationCodeID], [GLAcctID],[Explanation], [ExpensePaidTo]) VALUES (@WIPExpenseID,@EntryDate,@MatterID,@Quantity,@Rate,@TotalExpensed ,@ExplanationCodeID, @GLAcctID,@Explanation, @ExpensePaidTo)";
                                        command1.Parameters.AddWithValue("@WIPExpenseID", a.ID);
                                        command1.Parameters.AddWithValue("@EntryDate", PCLaw.General.EntryType.ToString());
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
                        }
                    }
                }




            }
            

        }

    }
}
