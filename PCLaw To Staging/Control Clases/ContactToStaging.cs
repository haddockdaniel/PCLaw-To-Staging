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
    public class ContactToStaging
    {

        public void insertIntoStaging(PLConvert.PCLawConversion PCLaw)
        {

            using (SqlConnection connection = new SqlConnection("Data Source=localhost;Initial Catalog=PCLawStg;Integrated Security=SSPI;"))
            {
                connection.Open();
                //int count = 1;
                while (PCLaw.Contact.GetNextRecord() == 0)
                {
                    if (PCLaw.Contact.Status == PLXMLData.eSTATUS.ACTIVE)
                    {
                        using (SqlCommand command1 = new SqlCommand())
                        {
                            command1.Connection = connection;
                            command1.CommandType = CommandType.Text;
                            command1.CommandText = "INSERT into Contact ([ContactID], [oldid], [ContactType], [nickname], [IsCorp], [CompanyName], [FirstName], [MiddleName], [LastName], [AddressLine1], [AddressLine2], [City], [State], [Zip], [Country], [BusPhone], [HomePhone], [CellPhone], [Email]) VALUES (@ContactID, @oldid, @ContactType, @nickname, @IsCorp, @CompanyName, @FirstName, @MiddleName, @LastName, @AddressLine1, @AddressLine2, @City, @State, @Zip, @Country, @BusPhone, @HomePhone, @CellPhone, @Email)";
                            command1.Parameters.AddWithValue("@ContactID", PCLaw.Contact.ID);
                            command1.Parameters.AddWithValue("@oldid", PCLaw.Contact.ID);
                            command1.Parameters.AddWithValue("@ContactType", PCLaw.Contact.MainContTypeID);
                            command1.Parameters.AddWithValue("@nickname", PCLaw.Contact.NickName);
                            if (string.IsNullOrEmpty(PCLaw.Contact.Name.Company))
                                command1.Parameters.AddWithValue("@iscorp", false);
                            else
                                command1.Parameters.AddWithValue("@iscorp", true);
                            command1.Parameters.AddWithValue("@companyname", PCLaw.Contact.Name.Company);
                            command1.Parameters.AddWithValue("@firstname", PCLaw.Contact.Name.First);
                            command1.Parameters.AddWithValue("@middlename", PCLaw.Contact.Name.Middle);
                            command1.Parameters.AddWithValue("@lastname", PCLaw.Contact.Name.Last);
                            command1.Parameters.AddWithValue("@addressline1", PCLaw.Contact.AddressMain.Addr1);
                            command1.Parameters.AddWithValue("@addressline2", PCLaw.Contact.AddressMain.Addr2);
                            command1.Parameters.AddWithValue("@City", PCLaw.Contact.AddressMain.City);
                            command1.Parameters.AddWithValue("@State", PCLaw.Contact.AddressMain.Prov);
                            command1.Parameters.AddWithValue("@Zip", PCLaw.Contact.AddressMain.Postal);
                            command1.Parameters.AddWithValue("@Country", PCLaw.Contact.AddressMain.Country);
                            command1.Parameters.AddWithValue("@busphone", PCLaw.Contact.Phone.BusPhone);
                            command1.Parameters.AddWithValue("@homephone", PCLaw.Contact.Phone.HomePhone);
                            command1.Parameters.AddWithValue("@cellphone", PCLaw.Contact.Phone.CellPhone);
                            command1.Parameters.AddWithValue("@email", PCLaw.Contact.Phone.BusEMail);
                            try
                            {

                                command1.ExecuteNonQuery();
                            }
                            catch (SqlException ex1)
                            {
                                MessageBox.Show(ex1.Message);
                            }

                        }

                        //count++;
                    }

                }
            }

        }
    }
}
