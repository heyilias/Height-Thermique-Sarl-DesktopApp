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
using System.Globalization;

namespace Height_Thermique_Sarl
{
    public partial class frm_STT : Form
    {
        public frm_STT()
        {
            InitializeComponent();
        }


        SqlConnection cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["connect"].ConnectionString);
        DataSet ds = new DataSet();
        SqlCommand cmd = new SqlCommand();
        SqlDataAdapter dapGetInfo = new SqlDataAdapter();
        SqlDataAdapter dapDevis = new SqlDataAdapter();
        SqlDataAdapter dapProduits = new SqlDataAdapter();
        DataRow ligne;
        SqlCommandBuilder cb;
        DataTable Dt = new DataTable();
        void CreateDataTable()
        {
            Dt.Columns.Add("N°");
            Dt.Columns.Add("Designation");
            Dt.Columns.Add("U");
            Dt.Columns.Add("QTE");
            Dt.Columns.Add("PU");
            Dt.Columns.Add("TOTAL");

            dgvallProdutcs.DataSource = Dt;
        }
        void resizeDGV()
        {
            dgvallProdutcs.RowHeadersWidth = 57;
            dgvallProdutcs.Columns[0].Width = 40;
            dgvallProdutcs.Columns[1].Width = 243;
            dgvallProdutcs.Columns[2].Width = 48;
            dgvallProdutcs.Columns[3].Width = 50;
            dgvallProdutcs.Columns[4].Width = 118;
            dgvallProdutcs.Columns[5].Width = 132;
        }

        void ClearBoxes()
        {
            txtNum.Text = "";
            txtNameProducts.Text = "";
            txtUnit.Text = "";
            txtQuantity.Text = "";
            txtPrice.Text = "";
            txtTotal.Text = "";

            btnChoose.Focus();
        }


        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void frm_STT_Load(object sender, EventArgs e)
        {
            CreateDataTable();
            resizeDGV();
            cmbCivilite.DropDownStyle = ComboBoxStyle.DropDownList;

            // Situation table 
            dapDevis = new SqlDataAdapter("select * from Situation", cnx);
            dapDevis.Fill(ds, "Situation");

            //Products table
            dapProduits = new SqlDataAdapter("select * from ProduitSTT", cnx);
            dapProduits.Fill(ds, "ProduitSTT");
        }

        private void btnChoose_Click(object sender, EventArgs e)
        {
            ClearBoxes();
            ProductsList frm = new ProductsList();
            frm.ShowDialog();
            frm.txtSearch.Focus();
            txtNum.Text = frm.dgvProducts.CurrentRow.Cells[0].Value.ToString();
            txtNameProducts.Text = frm.dgvProducts.CurrentRow.Cells[1].Value.ToString();
            txtUnit.Text = frm.dgvProducts.CurrentRow.Cells[2].Value.ToString();
            txtPrice.Text = frm.dgvProducts.CurrentRow.Cells[3].Value.ToString();

            txtQuantity.Focus();
        }

        private void txtQuantity_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != 8)
                e.Handled = true;
        }

        private void txtPrice_KeyPress(object sender, KeyPressEventArgs e)
        {
            char decimalSeparator = Convert.ToChar(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != 8 && e.KeyChar != decimalSeparator)
                e.Handled = true;
        }

        void CalculAmount()
        {
            if (txtQuantity.Text != string.Empty && txtPrice.Text != string.Empty)
                txtTotal.Text = (Convert.ToDouble(txtPrice.Text) * Convert.ToInt32(txtQuantity.Text)).ToString();
        }

        private void txtPrice_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && txtPrice.Text != string.Empty)
            {
                txtQuantity.Focus();
            }
        }

        private void txtPrice_KeyUp(object sender, KeyEventArgs e)
        {
            CalculAmount();
        }

        private void txtQuantity_KeyUp(object sender, KeyEventArgs e)
        {
            CalculAmount();
        }

        private void txtQuantity_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && txtQuantity.Text != "" && txtPrice.Text != "" && txtNameProducts.Text != "" && txtUnit.Text != "" && txtTotal.Text != "")
            {
                for (int i = 0; i < dgvallProdutcs.Rows.Count - 1; i++)
                {
                    if (dgvallProdutcs.Rows[i].Cells[0].Value.ToString() == txtNum.Text)
                    {
                        MessageBox.Show("Ce produit déja choisi", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }

                DataRow r = Dt.NewRow();
                r[0] = txtNum.Text;
                r[1] = txtNameProducts.Text;
                r[2] = txtUnit.Text;
                r[3] = txtQuantity.Text;
                r[4] = txtPrice.Text;
                r[5] = txtTotal.Text;

                Dt.Rows.Add(r);
                dgvallProdutcs.DataSource = Dt;

                ClearBoxes();

                txtTotalTTC.Text = (from DataGridViewRow row in dgvallProdutcs.Rows
                                    where row.Cells[5].FormattedValue.ToString() != string.Empty
                                    select Convert.ToDouble(row.Cells[5].FormattedValue)).Sum().ToString();
            }
        }

        private void dgvallProdutcs_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                txtNum.Text = dgvallProdutcs.CurrentRow.Cells[0].Value.ToString();
                txtNameProducts.Text = dgvallProdutcs.CurrentRow.Cells[1].Value.ToString();
                txtUnit.Text = dgvallProdutcs.CurrentRow.Cells[2].Value.ToString();
                txtQuantity.Text = dgvallProdutcs.CurrentRow.Cells[3].Value.ToString();
                txtPrice.Text = dgvallProdutcs.CurrentRow.Cells[4].Value.ToString();
                txtTotal.Text = dgvallProdutcs.CurrentRow.Cells[5].Value.ToString();

                dgvallProdutcs.Rows.RemoveAt(dgvallProdutcs.CurrentRow.Index);
                txtQuantity.Focus();
            }
            catch
            {
                return;
            }
        }

        private void dgvallProdutcs_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            txtTotalTTC.Text = (from DataGridViewRow row in dgvallProdutcs.Rows
                                where row.Cells[5].FormattedValue.ToString() != string.Empty
                                select Convert.ToDouble(row.Cells[5].FormattedValue)).Sum().ToString();

        }

        private void modifierToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dgvallProdutcs_DoubleClick(sender, e);
        }

        private void supprimerLigneSelectionneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dgvallProdutcs.Rows.RemoveAt(dgvallProdutcs.CurrentRow.Index);
        }

        private void supprimerTousLesLignesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dt.Rows.Clear();
            dgvallProdutcs.Refresh();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbCivilite.Text == "" || txtCltName.Text == "" || txtNumDevis.Text == "" || txtObjetDevis.Text == "" || txtSeller.Text == "")
                {
                    MessageBox.Show("Merci de remplir les champs !");
                    return;
                }

                ligne = ds.Tables["Situation"].NewRow();
                ligne[1] = txtNumDevis.Text;
                ligne[2] = txtObjetDevis.Text;
                ligne[3] = txtSeller.Text;
                ligne[4] = dtpDevis.Text;
                ligne[5] = cmbCivilite.Text;
                ligne[6] = txtCltName.Text;

                ds.Tables["Situation"].Rows.Add(ligne);
                cb = new SqlCommandBuilder(dapDevis);
                dapDevis.Update(ds, "Situation");

                //Add all products in tables 


                for (int i = 0; i < dgvallProdutcs.Rows.Count - 1; i++)
                {
                    ligne = ds.Tables["ProduitSTT"].NewRow();
                    //ligne[0] = dgvallProdutcs.Rows[i].Cells[0].Value.ToString();
                    ligne[1] = dgvallProdutcs.Rows[i].Cells[1].Value.ToString();
                    ligne[2] = dgvallProdutcs.Rows[i].Cells[2].Value.ToString();
                    ligne[3] = dgvallProdutcs.Rows[i].Cells[3].Value.ToString();
                    ligne[4] = dgvallProdutcs.Rows[i].Cells[4].Value.ToString();
                    ligne[5] = dgvallProdutcs.Rows[i].Cells[5].Value.ToString();
                    ligne[6] = txtNumDevis.Text;

                    ds.Tables["ProduitSTT"].Rows.Add(ligne);

                }

                cb = new SqlCommandBuilder(dapProduits);
                dapProduits.Update(ds, "ProduitSTT");

                MessageBox.Show("Ajouter avec succes");
                ClearBoxes();
                cmbCivilite.Text = "";
                txtCltName.Clear();
                txtObjetDevis.Clear();
                txtCltName.ReadOnly = true;
                cmbCivilite.Enabled = false;
                dtpDevis.Enabled = false;
                txtSeller.ReadOnly = true;
                txtNumDevis.ReadOnly = true;
                txtObjetDevis.ReadOnly = true;
                btnChoose.Enabled = false;

            }
            catch (Exception Ex)
            {
                MessageBox.Show("Erreur -- Details : " + Ex.Message, "Erreur !!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            txtNumDevis.Focus();
            txtCltName.ReadOnly = false;
            cmbCivilite.Enabled = true;
            dtpDevis.Enabled = true;
            txtSeller.ReadOnly = false;
            txtNumDevis.ReadOnly = false;
            txtObjetDevis.ReadOnly = false;
            btnChoose.Enabled = true;
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            dgvallProdutcs.Rows.RemoveAt(dgvallProdutcs.CurrentRow.Index);
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
                param.Value = txtNumDevis.Text;
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

        private void txtNameProducts_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && txtNameProducts.Text != string.Empty)
            {
                txtUnit.Focus();
            }
        }

        private void txtUnit_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && txtUnit.Text != string.Empty)
            {
                txtPrice.Focus();
            }
        }
    }
}
