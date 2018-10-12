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
    public class LawyerToStaging
    {
        public void insertIntoStaging(PLConvert.PCLawConversion PCLaw, List<string> lawyerList)
        {
            //int lawyerIndex = 0;
            using (SqlConnection connection = new SqlConnection("Data Source=localhost;Initial Catalog=PCLawStg;Integrated Security=SSPI;"))
            {
                connection.Open();
                while (PCLaw.Lawyer.GetNextRecord() == 0)
                {
                    int lawyerIndex = lawyerList.FindIndex(f => f.ToString() == PCLaw.Lawyer.ID.ToString());
                    if (lawyerIndex > -1) // it IS in the list of selected lawyers
                    {
                        double[] rates = new double[10];
                        int count = 0;
                        while (PCLaw.Rate.GetNextRecord() == 0)
                        {
                            if (count > 9) 
                                break;
                            rates[count] = PCLaw.Lawyer.GetRateAmountFromRateID(PCLaw.Rate.ID);
                            count++;
                        }

                        //add the lawyer and rates to staging

                        using (SqlCommand command = new SqlCommand())
                        {
                            command.Connection = connection;
                            command.CommandType = CommandType.Text;
                            command.CommandText = "INSERT into Lawyer (lawyerid, oldID, Name, NickName, Initials, rate1, rate2, rate3, rate4, rate5, rate6, rate7, rate8, rate9, rate10, active, classification) VALUES (@lawyerid, @oldID, @Name, @NickName, @Initials, @rate1, @rate2, @rate3, @rate4, @rate5, @rate6, @rate7, @rate8, @rate9, @rate10, @active, @classification)";
                            command.Parameters.AddWithValue("@lawyerID", PCLaw.Lawyer.ID);
                            command.Parameters.AddWithValue("@oldID", PCLaw.Lawyer.ID);
                            command.Parameters.AddWithValue("@Name", PCLaw.Lawyer.Name);
                            command.Parameters.AddWithValue("@NickName", PCLaw.Lawyer.NickName);
                            command.Parameters.AddWithValue("@Initials", PCLaw.Lawyer.Initials);
                            command.Parameters.AddWithValue("@active", getLawyerStatus(PCLaw.Lawyer.Status));
                            command.Parameters.AddWithValue("@classification", getClassificationID(PCLaw.Lawyer.Classification.ToString()));
                            command.Parameters.Add("@rate1", SqlDbType.Decimal).Value = rates[0];
                            command.Parameters.Add("@rate2", SqlDbType.Decimal).Value = rates[1];
                            command.Parameters.Add("@rate3", SqlDbType.Decimal).Value = rates[2];
                            command.Parameters.Add("@rate4", SqlDbType.Decimal).Value = rates[3];
                            command.Parameters.Add("@rate5", SqlDbType.Decimal).Value = rates[4];
                            command.Parameters.Add("@rate6", SqlDbType.Decimal).Value = rates[5];
                            command.Parameters.Add("@rate7", SqlDbType.Decimal).Value = rates[6];
                            command.Parameters.Add("@rate8", SqlDbType.Decimal).Value = rates[7];
                            command.Parameters.Add("@rate9", SqlDbType.Decimal).Value = rates[8];
                            command.Parameters.Add("@rate10", SqlDbType.Decimal).Value = rates[9];

                            try
                            {

                                command.ExecuteNonQuery();
                            }
                            catch (SqlException ex1)
                            {
                                MessageBox.Show("inner3: " + ex1.Message);
                            }
                        }
                    }
                }
            }


        }

        private int getClassificationID(string classification)
        {
            switch (classification)
            {
                case "SeniorPartner":
                    return 1;
                case "JrPartner":
                    return 2;
                case "Associate":
                    return 3;
                case "LawClerk":
                    return 4;
                case "Paralegal":
                    return 5;
                case "TimeKeeper":
                    return 6;
                default:
                    return 3;
            }

        }


        private bool getLawyerStatus(PLXMLData.eSTATUS status)
        {
            if (status.ToString().Equals("ACTIVE"))
                return true;
            else
                return false;

        }




    }
}
