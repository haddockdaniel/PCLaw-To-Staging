using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PLConvert;
using System.Data.SqlClient;
using System.IO;

namespace PCLaw_To_Staging
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            PCLaw = new PLConvert.PCLawConversion();
            PLConvert.PLLink.ResetLogFile();
            lawyerSorter = new ListViewColumnSorter();
            listViewLawyer.View = View.Details;
            listViewLawyer.ListViewItemSorter = lawyerSorter;
            listViewLawyer.Columns.Add("Name", 180);
            listViewLawyer.Columns.Add("ID", 35);

            //get all lawyers and list them in the box.
            while (PCLaw.Lawyer.GetNextRecord() == 0)
                if (PCLaw.Lawyer.Status == PLXMLData.eSTATUS.ACTIVE)
                {
                    listViewLawyer.Items.Add(new ListViewItem(new string[] { PCLaw.Lawyer.Name.Trim(), PCLaw.Lawyer.ID.ToString() }));
                }
        }

        public static PLConvert.PCLawConversion PCLaw;
        public List<string> lawyerList = new List<string>();
        List<int> matterList = new List<int>();
        public ListViewColumnSorter lawyerSorter;

        private void LawyerSelect_Click(object sender, EventArgs e)
        {
            //get selected lawyer ids
            int index = -1;
            ListView.SelectedIndexCollection indexes = this.listViewLawyer.SelectedIndices;
            foreach (int ind in indexes)
            {
                index = int.Parse(this.listViewLawyer.Items[ind].SubItems[1].Text);
                lawyerList.Add(index.ToString());
            }//end outer foreach


            
           // CheckTest ct = new CheckTest();
           // ct.insertIntoStaging(PCLaw, matterList);

            //be sure to mark the lawyers not to be added as inactive in the source sql db
            
            LawyerToStaging lts = new LawyerToStaging();
            lts.insertIntoStaging(PCLaw, lawyerList);

            UserToStaging uts = new UserToStaging();
            uts.insertIntoStaging(PCLaw);

            ToLToStaging tts = new ToLToStaging();
            tts.insertIntoStaging(PCLaw);

           ClientToStaging cts = new ClientToStaging();
           cts.insertIntoStaging(PCLaw);





           MatterToStaging mts = new MatterToStaging();
           mts.insertIntoStaging(PCLaw, lawyerList).ToList();


            
            //CntTypeToStaging ctts = new CntTypeToStaging();
            //ctts.insertIntoStaging(PCLaw);

           // ContactToStaging cnt = new ContactToStaging();
            //cnt.insertIntoStaging(PCLaw);

           // GLAcctToStaging glts = new GLAcctToStaging();
           // glts.insertIntoStaging(PCLaw);

            ExplCodeToStaging exp = new ExplCodeToStaging();
            exp.insertIntoStaging(PCLaw);

            TaskCodeToStaging task = new TaskCodeToStaging();
            task.insertIntoStaging(PCLaw);

            VendorToStaging vts = new VendorToStaging();
            vts.insertIntoStaging(PCLaw);
            
            //DiaryCodeToStaging dcts = new DiaryCodeToStaging();
          //  dcts.insertIntoStaging(PCLaw);

           // ApptToStaging apts = new ApptToStaging();
           // apts.insertIntoStaging(PCLaw);

           // PhoneToStaging pts = new PhoneToStaging();
           // pts.insertIntoStaging(PCLaw);

           // NoteToStaging nts = new NoteToStaging();
           // nts.insertIntoStaging(PCLaw);

           // MessageBox.Show("Close");
            

           // WIPFeeToStaging wfts = new WIPFeeToStaging();
          //  wfts.getTimeFrimPCLaw14(PCLaw);
           // wfts.createWIPFeeList(lawyerList);
           // wfts.insertIntoStaging(PCLaw);


           // WIPExpToStaging wets = new WIPExpToStaging();
           // wets.insertIntoStaging(PCLaw, matObjects);
           // MessageBox.Show("Check");

           // TrustToStaging trust = new TrustToStaging();
           // trust.generateTrust();
           // trust.insertIntoStaging(PCLaw);

            //MessageBox.Show("Close");

           // BillToStaging bts = new BillToStaging();
          //  bts.generateBillsandAllocs();
          //  bts.insertIntoStaging(PCLaw);


            //PaymentToStaging cts1 = new PaymentToStaging();
           // cts1.createPaymentList();
            //cts1.insertIntoStaging();

         //   GenRetainerToStaging grts = new GenRetainerToStaging();
          //  grts.createPaymentList();
         //   grts.insertIntoStaging();
//
           // WUDtoStaging wts = new WUDtoStaging();
          //  wts.insertIntoStaging();

            MessageBox.Show("Close");



        }


        private void buttonClearLawyers_Click(object sender, EventArgs e)
        {
            listViewLawyer.SelectedItems.Clear();
        }

        private void listViewLawyer_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column == lawyerSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (lawyerSorter.Order == System.Windows.Forms.SortOrder.Ascending)
                    lawyerSorter.Order = System.Windows.Forms.SortOrder.Descending;
                else
                    lawyerSorter.Order = System.Windows.Forms.SortOrder.Ascending;
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                lawyerSorter.SortColumn = e.Column;
                lawyerSorter.Order = System.Windows.Forms.SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            this.listViewLawyer.Sort();
        }
    }
}
