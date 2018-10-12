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
    public class MatterToStaging : StagingTable
    {
        //List<Matter> matList = new List<Matter>();
        public List<int> insertIntoStaging(PLConvert.PCLawConversion PCLaw, List<string> lawyerList)
        {
            List<int> matterList = new List<int>();
            using (SqlConnection connection = new SqlConnection("Data Source=localhost;Initial Catalog=PCLawStg;Integrated Security=SSPI;"))
            {
                connection.Open();
                while (PCLaw.Matter.GetNextRecord() == 0)
                {
                    if (PCLaw.Matter.Status == PLXMLData.eSTATUS.ACTIVE)
                    {
                        int lawyerIndex = lawyerList.FindIndex(f => f.ToString() == PCLaw.Matter.LwrRespID.ToString());
                        if (lawyerIndex > -1) // it IS in the list of selected lawyers
                        {
                            //matterList.Add(PCLaw.Matter.MatterID);
                            using (SqlCommand command1 = new SqlCommand())
                            {
                                command1.Connection = connection;
                                command1.CommandType = CommandType.Text;
                                command1.CommandText = "INSERT into Matter ([MatterID], [OldID], [Nickname], [MatterName], [RespLawyerID], [ClientID], [TypeOfLawID], [OpenDate], [RefLawyerName], [RateAmount], [BillNameFirst], [BillNameMiddle], [BillNameLast], [BIllNameCompany], [BillPhone], [BillFax], [IsCorp], [BIllAddressLine1], [BIllAddressLine2], [BIllCity], [BIllState], [BIllZip], [BIllCountry], [isActive], [Memo], [Description]) VALUES (@MatterID, @OldID, @Nickname, @MatterName, @RespLawyerID, @ClientID, @TypeOfLawID, @OpenDate, @RefLawyerName, @RateAmount, @BillNameFirst, @BillNameMiddle, @BillNameLast, @BIllNameCompany, @BillPhone, @BillFax, @IsCorp, @BIllAddressLine1, @BIllAddressLine2, @BIllCity, @BIllState, @BIllZip, @BIllCountry, @isActive, @Memo, @Description)";
                                command1.Parameters.AddWithValue("@MatterID", PCLaw.Matter.MatterID);
                                command1.Parameters.AddWithValue("@OldID", PCLaw.Matter.ID);
                                command1.Parameters.AddWithValue("@Nickname", PCLaw.Matter.NickName);
                                command1.Parameters.AddWithValue("@MatterName", PCLaw.Matter.Description);
                                command1.Parameters.AddWithValue("@RespLawyerID", PCLaw.Matter.LwrRespID);
                                command1.Parameters.AddWithValue("@ClientID", PCLaw.Matter.ClientID);
                                command1.Parameters.AddWithValue("@TypeOfLawID", PCLaw.Matter.TypeOfLawID);
                                command1.Parameters.AddWithValue("@OpenDate", PCLaw.Matter.DateOpened);
                                command1.Parameters.AddWithValue("@RefLawyerName", PCLaw.Matter.ReferredBy);
                                command1.Parameters.Add("@RateAmount", SqlDbType.Decimal).Value = PCLaw.Matter.RateAmount;
                                command1.Parameters.AddWithValue("@BillNameFirst", PCLaw.Matter.BillName1.First);
                                command1.Parameters.AddWithValue("@BillNameMiddle", PCLaw.Matter.BillName1.Middle);
                                command1.Parameters.AddWithValue("@BillNameLast", PCLaw.Matter.BillName1.Last);
                                command1.Parameters.AddWithValue("@BIllNameCompany", PCLaw.Matter.BillName1.Company);
                                command1.Parameters.AddWithValue("@BillPhone", PCLaw.Matter.BillPhone1.BusPhone);
                                command1.Parameters.AddWithValue("@BillFax", PCLaw.Matter.BillPhone1.BusFax);
                                if (string.IsNullOrEmpty(PCLaw.Matter.BillName1.Company))
                                    command1.Parameters.AddWithValue("@IsCorp", false);
                                else
                                    command1.Parameters.AddWithValue("@IsCorp", true);
                                command1.Parameters.AddWithValue("@BIllAddressLine1", PCLaw.Matter.BillAddress1.Addr1);
                                command1.Parameters.AddWithValue("@BIllAddressLine2", PCLaw.Matter.BillAddress1.Addr2);
                                command1.Parameters.AddWithValue("@BIllCity", PCLaw.Matter.BillAddress1.City);
                                command1.Parameters.AddWithValue("@BIllState", PCLaw.Matter.BillAddress1.Prov);
                                command1.Parameters.AddWithValue("@BIllZip", PCLaw.Matter.BillAddress1.Postal);
                                command1.Parameters.AddWithValue("@BIllCountry", PCLaw.Matter.BillAddress1.Country);
                                command1.Parameters.AddWithValue("@isActive", true);
                                command1.Parameters.AddWithValue("@Memo", PCLaw.Matter.Memos);
                                command1.Parameters.AddWithValue("@Description", PCLaw.Matter.Description);
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

            //remove clients not related to the right matters
           // removeUnneededClients();
            return matterList;

        }

        private void removeUnneededClients()
        {
            try
            {
                using (SqlConnection con = new SqlConnection("Data Source=localhost;Initial Catalog=PCLawStg;Integrated Security=SSPI;"))
                {
                    con.Open();
                    using (SqlCommand command = new SqlCommand("delete from client where clientid not in (select clientid from matter)", con))
                    {
                        command.ExecuteNonQuery();
                    }
                    con.Close();
                }
            }
            catch (SystemException ex)
            {
                MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));
            }

        }
/*
        public List<Matter> getMatterInfo(PLConvert.PCLawConversion PCLaw, List<string> matters)
        {
            while (PCLaw.Matter.GetNextRecord() == 0)
            {
                if (PCLaw.Matter.Status == PLXMLData.eSTATUS.ACTIVE)
                {
                    foreach (string mat in matters)
                    {
                        if (mat.Equals(PCLaw.Matter.NickName, StringComparison.OrdinalIgnoreCase))
                        {
                            Matter m = new Matter();
                            m.ID = PCLaw.Matter.MatterID;
                            m.shortname = PCLaw.Matter.NickName;
                            matList.Add(m);
                            break;
                        }
                    }
                }
            }

            return matList;
        }
 * */
    }
}
