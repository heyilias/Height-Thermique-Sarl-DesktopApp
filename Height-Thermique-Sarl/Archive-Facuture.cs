using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

namespace Height_Thermique_Sarl
{
    public partial class Archive_Facuture : Form
    {
        public Archive_Facuture()
        {
            InitializeComponent();
        }

        SqlConnection cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["connect"].ConnectionString);
        SqlCommand cmd = new SqlCommand();
        DataSet ds = new DataSet();
        SqlDataAdapter dap = new SqlDataAdapter();
        SqlDataAdapter dapGetInfo = new SqlDataAdapter();
        SqlDataAdapter dapSearch = new SqlDataAdapter();

        private void Archive_Facuture_Load(object sender, EventArgs e)
        {
            if (ds.Tables["FactureArchive"] != null)
                ds.Tables["FactureArchive"].Clear();

            dap = new SqlDataAdapter("select NumFacture as 'Facture N°',TitreCivilite +'. '+NomClient as 'Client',DateFacture as 'Date' from Facture", cnx);
            dap.Fill(ds, "FactureArchive");
            dgvDevisSells.DataSource = ds.Tables["FactureArchive"];
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            cmd = new SqlCommand("Search_FactureArchive", cnx);
            cmd.CommandType = CommandType.StoredProcedure;

            SqlParameter param = cmd.Parameters.Add("@wordSearch", SqlDbType.VarChar, 50);
            param.Value = txtSearch.Text;


            dapSearch = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            dapSearch.Fill(dt);

            dgvDevisSells.DataSource = dt;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                factureRPT report = new factureRPT();
                rptFacture frmReport = new rptFacture();

                cmd = new SqlCommand("GetFactureDetails", cnx);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter param = cmd.Parameters.Add("@NumFacture", SqlDbType.VarChar);
                param.Value = dgvDevisSells.CurrentRow.Cells[0].Value;
                dapGetInfo = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                dapGetInfo.Fill(dt);


                report.SetDataSource(dt);
                frmReport.crystalReportViewer1.ReportSource = report;
                frmReport.ShowDialog();
                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur -- Details : " + ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
