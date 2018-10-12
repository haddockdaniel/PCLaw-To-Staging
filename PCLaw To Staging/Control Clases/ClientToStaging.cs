using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Data.SqlTypes;
using System.Data.Sql;
using System.Data;
using PLConvert;

namespace PCLaw_To_Staging
{
    public class ClientToStaging : StagingTable
    {

        public void insertIntoStaging(PLConvert.PCLawConversion PCLaw)
        {

            using (SqlConnection connection = new SqlConnection("Data Source=localhost;Initial Catalog=PCLawStg;Integrated Security=SSPI;"))
            {
                connection.Open();
                int count = 1;
                while (PCLaw.Client.GetNextRecord() == 0)
                {
                    if (PCLaw.Client.Status == PLXMLData.eSTATUS.ACTIVE)
                    {
                        using (SqlCommand command1 = new SqlCommand())
                        {
                            command1.Connection = connection;
                            command1.CommandType = CommandType.Text;
                            command1.CommandText = "INSERT into Client ([ClientID], [oldid], [nickname], [IsCorp], [CompanyName], [FirstName], [MiddleName], [LastName], [Title], [Suffix], [AddressLine1], [AddressLine2], [City], [State], [Zip], [Country], [BusPhone], [HomePhone], [HomeFax], [BusFax], [Email], [Attention], [Website]) VALUES (@ClientID, @oldid, @nickname, @IsCorp, @CompanyName, @FirstName, @MiddleName, @LastName, @Title, @Suffix, @AddressLine1, @AddressLine2, @City, @State, @Zip, @Country, @BusPhone, @HomePhone, @HomeFax, @BusFax, @Email, @Attention, @Website)";
                            command1.Parameters.AddWithValue("@ClientID", PCLaw.Client.ID);
                            command1.Parameters.AddWithValue("@oldid", PCLaw.Client.ID);
                            command1.Parameters.AddWithValue("@nickname", PCLaw.Client.NickName);
                            command1.Parameters.AddWithValue("@iscorp", PCLaw.Client.Name.IsCorp);
                            command1.Parameters.AddWithValue("@companyname", PCLaw.Client.Name.Company);
                            command1.Parameters.AddWithValue("@firstname", PCLaw.Client.Name.First);
                            command1.Parameters.AddWithValue("@middlename", PCLaw.Client.Name.Middle);
                            command1.Parameters.AddWithValue("@lastname", PCLaw.Client.Name.Last);
                            command1.Parameters.AddWithValue("@Title", PCLaw.Client.Name.Title);
                            command1.Parameters.AddWithValue("@Suffix", PCLaw.Client.Name.Suffix);
                            command1.Parameters.AddWithValue("@addressline1", PCLaw.Client.Address.Addr1);
                            command1.Parameters.AddWithValue("@addressline2", PCLaw.Client.Address.Addr2);
                            command1.Parameters.AddWithValue("@City", PCLaw.Client.Address.City);
                            command1.Parameters.AddWithValue("@State", PCLaw.Client.Address.Prov);
                            command1.Parameters.AddWithValue("@Zip", PCLaw.Client.Address.Postal);
                            command1.Parameters.AddWithValue("@Country", PCLaw.Client.Address.Country);
                            command1.Parameters.AddWithValue("@busphone", PCLaw.Client.Phone.BusPhone);
                            command1.Parameters.AddWithValue("@homephone", PCLaw.Client.Phone.HomePhone);
                            command1.Parameters.AddWithValue("@homefax", PCLaw.Client.Phone.HomeFax);
                            command1.Parameters.AddWithValue("@busfax", PCLaw.Client.Phone.BusFax);
                            command1.Parameters.AddWithValue("@email", PCLaw.Client.Phone.BusEMail);
                            command1.Parameters.AddWithValue("@Attention", PCLaw.Client.Address.Attn);
                            command1.Parameters.AddWithValue("@Website", PCLaw.Client.Phone.WebPage);
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

                    count++;


                }
            }

        }


        private string getRidOfNulls(string data)
        {
            if (string.IsNullOrEmpty(data))
                return "";
            else
                return data;
        }

        public DataTable getAddressTable()
        {
            DataTable Table;
            string sSelect = "";
            int nClient = 0;

            Table = new DataTable("Addresses");

            sSelect = "SELECT [BilAdrSysNbr] ,[BilAdrName] ,[BilAdrAddress]  ,[BilAdrCity] ,[BilAdrState] ,[BilAdrZip] ,[BilAdrCountry] ,[BilAdrEmail] FROM [BillingAddress]";
            ReadAMTable(ref Table, sSelect);
            return Table;
        }

    }
}
