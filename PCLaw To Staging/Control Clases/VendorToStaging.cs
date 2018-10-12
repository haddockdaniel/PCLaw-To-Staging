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
    public class VendorToStaging
    {

        public void insertIntoStaging(PLConvert.PCLawConversion PCLaw)
        {

            using (SqlConnection connection = new SqlConnection("Data Source=localhost;Initial Catalog=PCLawStg;Integrated Security=SSPI;"))
            {
                connection.Open();
                while (PCLaw.Vendor.GetNextRecord() == 0)
                {
                    if (PCLaw.Vendor.Status == PLXMLData.eSTATUS.ACTIVE)
                    {
                        using (SqlCommand command1 = new SqlCommand())
                        {
                            command1.Connection = connection;
                            command1.CommandType = CommandType.Text;
                            command1.CommandText = "INSERT into Vendor ([VendorID] ,[OldID] ,[CompanyName] ,[FirstName] ,[MiddleName] ,[LastName] ,[AccountNumber] ,[isCorp] ,[Nickname] ,[AddressLine1] ,[AddressLine2] ,[City] ,[State] ,[Zip],[Country] ,[BusPhone] ,[BusFax] ,[HomePhone] ,[HomeFax] ,[Cell] ,[Email] ,[isActive] ,[Terms] ,[DiscountPercentage] ,[DiscountDays]) VALUES (@VendorID ,@OldID ,@CompanyName ,@FirstName ,@MiddleName ,@LastName ,@AccountNumber ,@isCorp ,@Nickname ,@AddressLine1 ,@AddressLine2 ,@City ,@State ,@Zip,@Country ,@BusPhone ,@BusFax ,@HomePhone ,@HomeFax ,@Cell ,@Email ,@isActive ,@Terms ,@DiscountPercentage ,@DiscountDays)";
                            command1.Parameters.AddWithValue("@VendorID", PCLaw.Vendor.ID);
                            command1.Parameters.AddWithValue("@OldID", PCLaw.Vendor.ID);
                            command1.Parameters.AddWithValue("@AccountNumber", PCLaw.Vendor.AcctNum);
                            command1.Parameters.AddWithValue("@nickname", PCLaw.Vendor.NickName);
                            command1.Parameters.AddWithValue("@companyname", PCLaw.Vendor.Name.Company);
                            if (string.IsNullOrEmpty(PCLaw.Vendor.Name.Company))
                                command1.Parameters.AddWithValue("@iscorp", false);
                            else
                                command1.Parameters.AddWithValue("@iscorp", true);
                            command1.Parameters.AddWithValue("@firstname", PCLaw.Vendor.Name.First);
                            command1.Parameters.AddWithValue("@middlename", PCLaw.Vendor.Name.Middle);
                            command1.Parameters.AddWithValue("@lastname", PCLaw.Vendor.Name.Last);
                            command1.Parameters.AddWithValue("@addressline1", PCLaw.Vendor.Address.Addr1);
                            command1.Parameters.AddWithValue("@addressline2", PCLaw.Vendor.Address.Addr2);
                            command1.Parameters.AddWithValue("@City", PCLaw.Vendor.Address.City);
                            command1.Parameters.AddWithValue("@State", PCLaw.Vendor.Address.Prov);
                            command1.Parameters.AddWithValue("@Zip", PCLaw.Vendor.Address.Postal);
                            command1.Parameters.AddWithValue("@Country", PCLaw.Vendor.Address.Country);
                            command1.Parameters.AddWithValue("@busphone", PCLaw.Vendor.Phone.BusPhone);
                            command1.Parameters.AddWithValue("@homephone", PCLaw.Vendor.Phone.HomePhone);
                            command1.Parameters.AddWithValue("@BusFax", PCLaw.Vendor.Phone.BusFax);
                            command1.Parameters.AddWithValue("@HomeFax", PCLaw.Vendor.Phone.HomeFax);
                            command1.Parameters.AddWithValue("@cell", PCLaw.Vendor.Phone.CellPhone);
                            command1.Parameters.AddWithValue("@email", PCLaw.Vendor.Phone.BusEMail);
                            command1.Parameters.AddWithValue("@isActive", true);
                            command1.Parameters.AddWithValue("@Terms", PCLaw.Vendor.Terms);
                            command1.Parameters.Add("@DiscountPercentage", SqlDbType.Decimal).Value = PCLaw.Vendor.DiscPct1;
                            command1.Parameters.AddWithValue("@DiscountDays", PCLaw.Vendor.DiscDays1);
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
