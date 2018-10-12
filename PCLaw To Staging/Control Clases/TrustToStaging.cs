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
    public class TrustToStaging
    {
        List<Trust> trustList = new List<Trust>();
        Trust trust;
        public void generateTrust()
        {

            string sqlBill = @"SELECT  [TBankAllocInfoCheckID] ,[MatterID] ,[TBankAllocInfAllocID] ,[TBankAllocInfoAmount] ,[TBankCommInfEntryType] ,[TBankAllocInfExplanation], tbcomm.TBankCommInfDate,tbcomm.TBankCommInfPaidTo, tbcomm.TBankCommInfCheck FROM [TBAlloc] inner join tbcomm on TBankAllocInfoCheckID = tBankCommInfSequenceID where TBankCommInfStatus = 0 and TBankAllocInfoStatus = 0 and tballoc.MatterID in (select matterid from MattInf where MatterInfoStatus = 0)";

            SqlConnection con = new SqlConnection("Data Source=localhost;Initial Catalog=PCLAWDB_06769;Integrated Security=SSPI;");

            using (var command = new SqlCommand(sqlBill, con))
            {
                con.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        trust = new Trust();
                        trust.amount = double.Parse(reader["TBankAllocInfoAmount"].ToString().Trim());

                        switch (reader["TBankCommInfEntryType"].ToString().Trim())
                        {
                            case "2050": //Reciept
                                trust.entryType = 0;
                                break;
                            case "2049": //check
                                trust.entryType = 1;
                                break;
                            case "2051": //opening balance
                                trust.entryType = 2;
                                break;
                            case "2052": //Trust to trust transfer
                                trust.entryType = 3;
                                break;
                            case "2053": //trust_tdt
                                trust.entryType = 4;
                                break;
                        }

                        trust.paymentType = 0;
                        trust.tbAcct = "1";
                        trust.paidTo = reader["TBankCommInfPaidTo"].ToString().Trim();
                        trust.date = reader["TBankCommInfDate"].ToString().Trim();
                        trust.matter = reader["MatterID"].ToString().Trim();
                        trust.explanation = reader["TBankAllocInfExplanation"].ToString().Trim();
                        trust.checkNo = reader["TBankCommInfCheck"].ToString().Trim();
                        trustList.Add(trust);
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
                foreach (Trust t in trustList)
                {

                            using (SqlCommand command1 = new SqlCommand())
                            {
                                command1.Connection = connection;
                                command1.CommandType = CommandType.Text;
                                command1.CommandText = "INSERT into [Trust] ([Amount],[EntryType],[PaymentType],[Date],[CheckNum],[GLAcctID],[PaidTo],[MatterID],[Explanation]) VALUES (@Amount,@EntryType,@PaymentType,@Date,@CheckNum,@GLAcctID,@PaidTo,@MatterID,@Explanation)";
                                command1.Parameters.AddWithValue("@Date", t.date);
                                command1.Parameters.AddWithValue("@MatterID", t.matter);
                                command1.Parameters.AddWithValue("@GLAcctID", t.tbAcct);
                                command1.Parameters.AddWithValue("@Explanation", t.explanation);
                                command1.Parameters.AddWithValue("@EntryType", t.entryType);
                                command1.Parameters.AddWithValue("@PaymentType", "0");
                                command1.Parameters.AddWithValue("@CheckNum", t.checkNo);
                                command1.Parameters.AddWithValue("@PaidTo", t.paidTo);
                                command1.Parameters.Add("@Amount", SqlDbType.Decimal).Value = t.amount;

                                try
                                {
                                    command1.ExecuteNonQuery();
                                }
                                catch (SqlException ex1)
                                {
                                    MessageBox.Show(ex1.Message);
                                }

                            }
                        //}
                        count++;
                    
                        
                }
            }

        }

        private int getEntryType(PLTBEnt.eTBEntryType type)
        {
            switch (type.ToString().Trim())
            {
                case "Recipt":
                    return 0;
                case "Check":
                    return 1;
                case "TrustCOB":
                    return 2;
                case "TRUST_TDT":
                    return 3;
                case "TrustToTrustTransfer":
                    return 4;
                default:
                    MessageBox.Show(type.ToString().Trim());
                    return 5;
            }

        }
    }
}
