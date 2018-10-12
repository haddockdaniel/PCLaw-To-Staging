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
    public class GLAcctToStaging
    {

        public void insertIntoStaging(PLConvert.PCLawConversion PCLaw)
        {
            int count = 1;
            using (SqlConnection connection = new SqlConnection("Data Source=localhost;Initial Catalog=PCLawStg;Integrated Security=SSPI;"))
            {
                connection.Open();
                List<GLAcct> glaccts = new List<GLAcct>();
                GLAcct gl;

                //get the gb and tb accounts so we dont add them twice
                //gen bank
                while (PCLaw.GBAcct.GetNextRecord() == 0)
                {
                    gl = new GLAcct();
                    gl.GLName = PCLaw.GBAcct.BankName;
                    gl.BankType = "G";
                    glaccts.Add(gl);
                }


                //trust bank
                while (PCLaw.TBAcct.GetNextRecord() == 0)
                {
                    gl = new GLAcct();
                    gl.GLName = PCLaw.TBAcct.BankName;
                    gl.BankType = "T";
                    glaccts.Add(gl);
                }

                //add the gl accounts minus the bank accounts
                while (PCLaw.GLAccts.GetNextRecord() == 0)
                {
                    int index = 0;
                    index = glaccts.FindIndex(f => f.GLName == PCLaw.GLAccts.Name.ToString());
                    if (index < 0) // it IS NOT in the list of bank accounts so we add it. Bank accounts are automatically added as GL accounts when added above
                    {
                        using (SqlCommand command1 = new SqlCommand())
                        {
                            command1.Connection = connection;
                            command1.CommandType = CommandType.Text;
                            command1.CommandText = "INSERT into GLAcct ([GLAcctID], [oldid], [nickname], [GLName], [BankAccountNumber], [GLType], [BankType]) VALUES (@GLAcctID, @oldid, @nickname, @GLName, @BankAccountNumber, @GLType, @BankType)";
                            command1.Parameters.AddWithValue("@GLAcctID", count);
                            command1.Parameters.AddWithValue("@oldid", PCLaw.GLAccts.ID);
                            command1.Parameters.AddWithValue("@nickname", PCLaw.GLAccts.NickName);
                            command1.Parameters.AddWithValue("@GLName", PCLaw.GLAccts.Name);
                            command1.Parameters.AddWithValue("@BankAccountNumber", "");
                            command1.Parameters.AddWithValue("@GLType", getAcctType(PCLaw.GLAccts.AcctType));
                            command1.Parameters.AddWithValue("@BankType", "");
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

                //add the bank accounts
                //gen bank
                while (PCLaw.GBAcct.GetNextRecord() == 0)
                {
                    bool found = false;
                    using (SqlCommand command1 = new SqlCommand())
                    {
                        command1.Connection = connection;
                        command1.CommandType = CommandType.Text;
                        command1.CommandText = "INSERT into GLAcct ([GLAcctID], [oldid], [nickname], [GLName], [BankAccountNumber], [GLType], [BankType]) VALUES (@GLAcctID, @oldid, @nickname, @GLName, @BankAccountNumber, @GLType, @BankType)";
                        command1.Parameters.AddWithValue("@GLAcctID", count);
                        command1.Parameters.AddWithValue("@oldid", PCLaw.GBAcct.ID);
                        while (PCLaw.GLAccts.GetNextRecord() == 0)
                        {
                            if (PCLaw.GLAccts.Name.Equals(PCLaw.GBAcct.BankName))
                            {
                                found = true;
                                command1.Parameters.AddWithValue("@nickname", PCLaw.GLAccts.NickName);
                                break;
                            }
                        }
                        if (!found)
                            command1.Parameters.AddWithValue("@nickname", PCLaw.GBAcct.NickName);
                        command1.Parameters.AddWithValue("@GLName", PCLaw.GBAcct.BankName);
                        command1.Parameters.AddWithValue("@BankAccountNumber", PCLaw.GBAcct.BankAcctNumber);
                        command1.Parameters.AddWithValue("@GLType", 1);
                        command1.Parameters.AddWithValue("@BankType", "G");
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


                //trust bank
                while (PCLaw.TBAcct.GetNextRecord() == 0)
                {
                    bool found = false;
                    using (SqlCommand command1 = new SqlCommand())
                    {
                        command1.Connection = connection;
                        command1.CommandType = CommandType.Text;
                        command1.CommandText = "INSERT into GLAcct ([GLAcctID], [oldid], [nickname], [GLName], [BankAccountNumber], [GLType], [BankType]) VALUES (@GLAcctID, @oldid, @nickname, @GLName, @BankAccountNumber, @GLType, @BankType)";
                        command1.Parameters.AddWithValue("@GLAcctID", count);
                        command1.Parameters.AddWithValue("@oldid", PCLaw.TBAcct.ID);
                        while (PCLaw.GLAccts.GetNextRecord() == 0)
                        {
                            if (PCLaw.GLAccts.Name.Equals(PCLaw.TBAcct.BankName))
                            {
                                found = true;
                                command1.Parameters.AddWithValue("@nickname", PCLaw.GLAccts.NickName);
                                break;
                            }
                        }
                        if (!found)
                            command1.Parameters.AddWithValue("@nickname", PCLaw.TBAcct.NickName);
                        command1.Parameters.AddWithValue("@GLName", PCLaw.TBAcct.BankName);
                        command1.Parameters.AddWithValue("@BankAccountNumber", PCLaw.TBAcct.BankAcctNumber);
                        command1.Parameters.AddWithValue("@GLType", 1);
                        command1.Parameters.AddWithValue("@BankType", "T");
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

        public int getAcctType(PLGLAccts.eACCOUNT_TYPE type)
        {
            switch (type.ToString().Trim())
            {
                case "CURRENT_ASSET":
                    return 1;
                case "SHORT_LIABILITY":
                    return 2;
                case "EQUITY":
                    return 3;
                case "INCOME":
                    return 4;
                case "EXPENSE":
                    return 5;
                case "LONG_LIABILITY":
                    return 6;
                case "FIXED_ASSET":
                    return 7;
                default:
                    return 3;
            }//end switch
        }
    }
}
