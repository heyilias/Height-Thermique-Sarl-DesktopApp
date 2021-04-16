using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Data.SqlClient;

namespace Height_Thermique_Sarl
{
    public partial class Archive_Devis : Form
    {
        public Archive_Devis()
        {
            InitializeComponent();
        }

        SqlConnection cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["connect"].ConnectionString);
        SqlCommand cmd = new SqlCommand();
        DataSet ds = new DataSet();
        SqlDataAdapter dap = new SqlDataAdapter();
        SqlDataAdapter dapGetInfo = new SqlDataAdapter();
        SqlDataAdapter dapSearch = new SqlDataAdapter();

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                CRDevis report = new CRDevis();
                CryDevis frmReport = new CryDevis();

                cmd = new SqlCommand("GetDevisDetails", cnx);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter param = cmd.Parameters.Add("@NumDevis", SqlDbType.VarChar);
                param.Value = dgvDevisSells.CurrentRow.Cells[0].Value;
                dapGetInfo = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                dapGetInfo.Fill(dt);

                report.SetDataSource(dt);
                frmReport.crystalReportViewer1.ReportSource = report;
                frmReport.ShowDialog();
                this.Cursor = Cursors.Default;
            }
            catch (Exception Ex)
            {
                MessageBox.Show("Erreur -- Details : " + Ex.Message, "Erreur !!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Archive_Devis_Load(object sender, EventArgs e)
        {
            if (ds.Tables["DevisArchive"] != null)
                ds.Tables["DevisArchive"].Clear();

            dap = new SqlDataAdapter("select NumDevis as 'Devis N°', Objet as 'Objet', TitreCivilite +'. '+NomClient as 'Client', DateDevis as 'Date' from Devis", cnx);
            dap.Fill(ds, "DevisArchive");
            dgvDevisSells.DataSource = ds.Tables["DevisArchive"];
        }

        private void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            cmd = new SqlCommand("Search_DevisArchive", cnx);
            cmd.CommandType = CommandType.StoredProcedure;

            SqlParameter param = cmd.Parameters.Add("@wordSearch", SqlDbType.VarChar, 50);
            param.Value = txtSearch.Text;


            dapSearch = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            dapSearch.Fill(dt);

            dgvDevisSells.DataSource = dt;
        }
    }
}
