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
    public class PaymentToStaging
    {
        List<Payment> paymentList = new List<Payment>();
        List<GenRetainer> gretList = new List<GenRetainer>();

        public void createPaymentList()
        {
            Payment payment;
            GenRetainer genRet;
            string sql = @"SELECT * FROM [GBAlloc] inner join gbcomm on GBankCommInfID = GBankAllocInfCheckID inner join arinv on ARInvoiceInvNumber = GBankCommInfInvoice where  GBankAllocInfStatus = 0 and GBankAllocInfEntryType = 1101";
            SqlConnection con = new SqlConnection("Data Source=localhost;Initial Catalog=PCLAWDB_69606;Integrated Security=SSPI;");
            using (var command = new SqlCommand(sql, con))
            {
                con.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        try
                        {
                                payment = new Payment();
                                payment.ID = int.Parse(reader["GBankAllocInfAllocID"].ToString().Trim());

                                payment.date = reader["GBankCommInfDate"].ToString().Trim();
                                payment.client = "";
                                payment.glAcct = "";
                                payment.paymentClass = 0;


                                payment.bankAcctID = reader["GBankCommInfAccountID"].ToString().Trim();
                                payment.checkNo = reader["GBankCommInfClientCheck"].ToString().Trim();
                                payment.transID = reader["GBankCommInfCheck"].ToString().Trim();
                                payment.received = double.Parse(reader["GBankAllocInfAmount"].ToString().Trim());
                                payment.applied = double.Parse(reader["GBankAllocInfAmount"].ToString().Trim());
                                payment.invoice = reader["InvoiceID"].ToString().Trim();
                                payment.explanation = reader["GBankAllocInfExplanation"].ToString().Trim();
                                payment.oldID = reader["GBankAllocInfAllocID"].ToString().Trim();
                                payment.paymentType = 0;
                                payment.matter = reader["matterid"].ToString().Trim();
                                paymentList.Add(payment);


                        }
                        catch (Exception inner1)
                        { MessageBox.Show("Inner: " + inner1.Message); }
                    }
                }
            }

            sql = @"SELECT * FROM [GBAlloc] inner join gbcomm on GBankCommInfID = GBankAllocInfCheckID  where  GBankAllocInfStatus = 0 and GBankAllocInfEntryType = 1101 and GBankCommInfInvoice = ''";
            using (var command = new SqlCommand(sql, con))
            {
                //con.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        try
                        {

                                genRet = new GenRetainer();
                                genRet.ID = int.Parse(reader["GBankAllocInfAllocID"].ToString().Trim());
                                genRet.date = reader["GBankCommInfDate"].ToString().Trim();
                                genRet.checkNo = reader["GBankCommInfCheck"].ToString().Trim();
                                genRet.invoiceID = "";
                                genRet.bankAccount = reader["GBankCommInfAccountID"].ToString().Trim();
                                genRet.explanation = reader["GBankAllocInfExplanation"].ToString().Trim();
                                genRet.matterID = reader["matterid"].ToString().Trim();
                                genRet.payment = double.Parse(reader["GBankAllocInfAmount"].ToString().Trim());
                                gretList.Add(genRet);
                            

                        }
                        catch (Exception inner1)
                        { MessageBox.Show("Inner: " + inner1.Message); }
                    }
                }
            }

        }

        public void insertIntoStaging()
        {
            //payments
            using (SqlConnection connection = new SqlConnection("Data Source=localhost;Initial Catalog=PCLawStg;Integrated Security=SSPI;"))
            {
                connection.Open();

                foreach (Payment c in paymentList)
                {
                    using (SqlCommand command1 = new SqlCommand())
                    {

                        command1.Connection = connection;
                        command1.CommandType = CommandType.Text;
                        command1.CommandText = "INSERT into [Payment] ([PaymentID],[OldID],[BankAccountID],[CheckNumber],[TransactionID],[EntryDate],[PaymentReceived],[PaymentApplied],[GLAcct],[ClientID],[PaymentType],[MatterID],[PaymentClass],[InvoiceID],[Explanation]) VALUES (@PaymentID,@OldID,@BankAccountID,@CheckNumber,@TransactionID,@EntryDate,@PaymentReceived,@PaymentApplied,@GLAcct,@ClientID,@PaymentType,@MatterID,@PaymentClass,@InvoiceID,@Explanation)";
                        command1.Parameters.AddWithValue("@PaymentID", c.ID);
                        command1.Parameters.AddWithValue("@EntryDate", c.date);
                        command1.Parameters.AddWithValue("@GLAcct", c.glAcct);
                        command1.Parameters.AddWithValue("@OldID", c.oldID);
                        command1.Parameters.AddWithValue("@BankAccountID", c.bankAcctID);
                        command1.Parameters.AddWithValue("@CheckNumber", c.checkNo);
                        command1.Parameters.AddWithValue("@TransactionID", c.transID);
                        command1.Parameters.AddWithValue("@ClientID", c.client);
                        command1.Parameters.AddWithValue("@PaymentType", c.paymentType);
                        command1.Parameters.AddWithValue("@MatterID", c.matter);
                        command1.Parameters.AddWithValue("@PaymentClass", c.paymentClass);
                        command1.Parameters.AddWithValue("@InvoiceID", c.invoice);
                        command1.Parameters.AddWithValue("@Explanation", c.explanation);
                        command1.Parameters.Add("@PaymentReceived", SqlDbType.Decimal).Value = c.received;
                        command1.Parameters.Add("@PaymentApplied", SqlDbType.Decimal).Value = c.applied;
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

            //gen ret
            using (SqlConnection connection = new SqlConnection("Data Source=localhost;Initial Catalog=PCLawStg;Integrated Security=SSPI;"))
            {
                connection.Open();

                foreach (GenRetainer c in gretList)
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
