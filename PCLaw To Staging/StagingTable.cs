using System;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace PCLaw_To_Staging
{
    public abstract class StagingTable : object
    {
        public StagingTable()
        {

        }

        public string sAMServer;
        public void ReadAMTable(ref DataTable Table, string sSelect)
        {
            string sConn = string.Empty;

            if (Table == null)
                Table = new DataTable();
            try
            {
                sConn = @"Data Source=localhost;Initial Catalog=Juris5573000;Integrated Security=SSPI;";
                //sConn = @"Data Source=npc365;Initial Catalog=Amicus;Integrated Security=True;";
                //SqlConnection Conn = new SqlConnection(sConn);
                //Conn.Open();
                SqlDataAdapter Adapter = new SqlDataAdapter(sSelect, sConn);//Conn);
                //Conn.Close();
                Adapter.Fill(Table);
            }

            catch (Exception objError)
            {
                //write error to the windows event log                  
                //WriteToEventLog(objError);
                string sError = objError.ToString();
                //PLXMLLnk_LinkLog_CloseLog   (); 
                //PLXMLLnk_LinkLog_Show       ();  
                System.Diagnostics.Debug.Assert(false);
            }
        }

        public override string ToString()
        {
            return base.ToString();
        }


    }
}
