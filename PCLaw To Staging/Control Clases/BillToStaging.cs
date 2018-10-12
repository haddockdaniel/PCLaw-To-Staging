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
    public class BillToStaging
    {

        List<Bill> bills = new List<Bill>();
        Bill bill;

        public void generateBillsandAllocs()
        {
            string sqlBill = @"SELECT  [InvoiceID] ,[MatterID] ,[ARInvoiceInvNumber] ,[ARInvoiceDate] ,[ARInvoiceFees] ,[ARInvoiceHours] ,[ARInvoiceDisbs] ,[ARInvoiceGSTFees] ,[ARInvoiceGSTDisbs] ,[ARInvoiceCollLwyrID],[ARInvoiceTypeofLaw] FROM [ARInv]";
            
            SqlConnection con = new SqlConnection("Data Source=localhost;Initial Catalog=PCLAWDB_69606;Integrated Security=SSPI;");
            
            
            using (var command = new SqlCommand(sqlBill, con))
            {
                con.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        bill = new Bill();
                        bill.billNo = reader["ARInvoiceInvNumber"].ToString().Trim();
                        bill.date = reader["ARInvoiceDate"].ToString().Trim();
                        bill.disb = double.Parse(reader["ARInvoiceDisbs"].ToString().Trim());
                        bill.fees = double.Parse(reader["ARInvoiceFees"].ToString().Trim());
                        bill.hours = double.Parse(reader["ARInvoiceHours"].ToString().Trim()) / 3600;
                        bill.ID = int.Parse(reader["InvoiceID"].ToString().Trim());
                        bill.interest = 0;
                        bill.matter = reader["MatterID"].ToString().Trim();
                        bill.oldID = reader["InvoiceID"].ToString().Trim();
                        bill.respLawyer = reader["ARInvoiceCollLwyrID"].ToString().Trim();
                        bill.taxes = double.Parse(reader["ARInvoiceGSTFees"].ToString().Trim()) + double.Parse(reader["ARInvoiceGSTDisbs"].ToString().Trim());
                        bill.typeOfLaw = reader["ARInvoiceTypeofLaw"].ToString().Trim();
                        bills.Add(bill);
                    }
                }
            }
            con.Close();




        }


        public void insertIntoStaging(PLConvert.PCLawConversion PCLaw)
        {

            using (SqlConnection connection = new SqlConnection("Data Source=localhost;Initial Catalog=PCLawStg;Integrated Security=SSPI;"))
            {
                connection.Open();
                int count = 1;
                foreach (Bill invoice in bills)
                {

                        //add bill
                        using (SqlCommand command1 = new SqlCommand())
                        {
                            command1.Connection = connection;
                            command1.CommandType = CommandType.Text;
                            command1.CommandText = "INSERT into [Bill] ([BillID],[OldID],[BillNumber],[Date],[MatterID],[LawyerID],[Fees],[Disb],[Hours], [Taxes], [Interest]) VALUES (@BillID,@OldID,@BillNumber,@Date,@MatterID,@LawyerID,@Fees,@Disb,@Hours, @Taxes, @Interest)";
                            command1.Parameters.AddWithValue("@BillID", invoice.ID);
                            command1.Parameters.AddWithValue("@Date", invoice.date);
                            command1.Parameters.AddWithValue("@MatterID", invoice.matter);
                            command1.Parameters.AddWithValue("@OldID", invoice.oldID);
                            command1.Parameters.AddWithValue("@BillNumber", invoice.billNo);
                            command1.Parameters.AddWithValue("@LawyerID", invoice.respLawyer);
                            command1.Parameters.Add("@Fees", SqlDbType.Decimal).Value = invoice.fees;
                            command1.Parameters.Add("@Disb", SqlDbType.Decimal).Value = invoice.disb;
                            command1.Parameters.Add("@Hours", SqlDbType.Decimal).Value = invoice.hours;
                            command1.Parameters.Add("@Taxes", SqlDbType.Decimal).Value = invoice.taxes;
                            command1.Parameters.Add("@Interest", SqlDbType.Decimal).Value = 0.00;
                            try
                            {
                                command1.ExecuteNonQuery();
                            }
                            catch (SqlException ex1)
                            {
                                MessageBox.Show(ex1.Message);
                            }

                        }

                        //add alloc for time
                        if (invoice.fees != 0.00)
                        {
                            using (SqlCommand command1 = new SqlCommand())
                            {
                                command1.Connection = connection;
                                command1.CommandType = CommandType.Text;
                                command1.CommandText = "INSERT into [Allocation] ([AllocationID],[OldID],[Total],[QuantityHours],[Rate],[LawyerID],[GLAcctID],[EntryDate],[BillID],[CodeID],[TimeExp],[Description]) VALUES (@AllocationID,@OldID,@Total,@QuantityHours,@Rate,@LawyerID,@GLAcctID,@EntryDate,@BillID,@CodeID,@TimeExp,@Description)";
                                command1.Parameters.AddWithValue("@AllocationID", count);
                                command1.Parameters.AddWithValue("@EntryDate", invoice.date);
                                command1.Parameters.AddWithValue("@GLAcctID", 0);
                                command1.Parameters.AddWithValue("@OldID", invoice.ID);
                                command1.Parameters.AddWithValue("@BillID", invoice.ID);
                                command1.Parameters.AddWithValue("@LawyerID", invoice.respLawyer);
                                command1.Parameters.AddWithValue("@TimeExp", 0);
                                command1.Parameters.AddWithValue("@CodeID", 0);
                                command1.Parameters.AddWithValue("@Description", "Converted Invoice Allocation");
                                command1.Parameters.Add("@Total", SqlDbType.Decimal).Value = invoice.fees;
                                command1.Parameters.Add("@QuantityHours", SqlDbType.Decimal).Value = 1.00;
                                command1.Parameters.Add("@Rate", SqlDbType.Decimal).Value = invoice.fees;
                                count++;
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
                    //add expense allocs
                        if (invoice.disb != 0.00)
                        {
                            using (SqlCommand command1 = new SqlCommand())
                            {
                                command1.Connection = connection;
                                command1.CommandType = CommandType.Text;
                                command1.CommandText = "INSERT into [Allocation] ([AllocationID],[OldID],[Total],[QuantityHours],[Rate],[LawyerID],[GLAcctID],[EntryDate],[BillID],[CodeID],[TimeExp],[Description]) VALUES (@AllocationID,@OldID,@Total,@QuantityHours,@Rate,@LawyerID,@GLAcctID,@EntryDate,@BillID,@CodeID,@TimeExp,@Description)";
                                command1.Parameters.AddWithValue("@AllocationID", count);
                                command1.Parameters.AddWithValue("@EntryDate", invoice.date);
                                command1.Parameters.AddWithValue("@GLAcctID", 0);
                                command1.Parameters.AddWithValue("@OldID", invoice.ID);
                                command1.Parameters.AddWithValue("@BillID", invoice.ID);
                                command1.Parameters.AddWithValue("@LawyerID", invoice.respLawyer);
                                command1.Parameters.AddWithValue("@TimeExp", 1);
                                command1.Parameters.AddWithValue("@CodeID", 0);
                                command1.Parameters.AddWithValue("@Description", "Converted Invoice Allocation");
                                command1.Parameters.Add("@Total", SqlDbType.Decimal).Value = invoice.disb;
                                command1.Parameters.Add("@QuantityHours", SqlDbType.Decimal).Value = 1;
                                command1.Parameters.Add("@Rate", SqlDbType.Decimal).Value = invoice.disb;
                                count++;
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
}


