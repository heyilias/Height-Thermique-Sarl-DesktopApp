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
    public partial class Archive_Situation : Form
    {
        public Archive_Situation()
        {
            InitializeComponent();
        }

        SqlConnection cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["connect"].ConnectionString);
        SqlCommand cmd = new SqlCommand();
        DataSet ds = new DataSet();
        SqlDataAdapter dap = new SqlDataAdapter();
        SqlDataAdapter dapGetInfo = new SqlDataAdapter();
        SqlDataAdapter dapSearch = new SqlDataAdapter();

        private void Archive_Situation_Load(object sender, EventArgs e)
        {
            if (ds.Tables["STTArchive"] != null)
                ds.Tables["STTArchive"].Clear();

            dap = new SqlDataAdapter("select NumSTT as 'Situation N°', Objet as 'Objet', TitreCivilite +'. '+NomClient as 'Client', DateSTT as 'Date' from Situation", cnx);
            dap.Fill(ds, "STTArchive");
            dgvDevisSells.DataSource = ds.Tables["STTArchive"];
        }

        private void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            cmd = new SqlCommand("Search_STTArchive", cnx);
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
                rptSituation report = new rptSituation();
                CrySituation frmReport = new CrySituation();

                cmd = new SqlCommand("GetSTTDetails", cnx);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter param = cmd.Parameters.Add("@NumSTT", SqlDbType.VarChar);
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

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
