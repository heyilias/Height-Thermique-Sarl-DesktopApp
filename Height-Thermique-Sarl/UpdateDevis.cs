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
    public partial class UpdateDevis : Form
    {
        public UpdateDevis()
        {
            InitializeComponent();
        }

        SqlConnection cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["connect"].ConnectionString);
        SqlDataAdapter dap = new SqlDataAdapter();
        SqlDataAdapter dapProduits = new SqlDataAdapter();
        SqlDataAdapter dapGetInfo = new SqlDataAdapter();
        SqlCommand cmd = new SqlCommand();
        DataSet ds = new DataSet();
        DataTable Dt = new DataTable();
        DataRow ligne;
        
        SqlCommandBuilder cb = new SqlCommandBuilder();

        void ClearBoxes()
        {
            txtNum.Text = "";
            txtNameProducts.Text = "";
            txtUnit.Text = "";
            txtQuantity.Text = "";
            txtPrice.Text = "";
            txtTotal.Text = "";

        }

        void CreateDataTable()
        {
            Dt.Columns.Add("N°");
            Dt.Columns.Add("Designation");
            Dt.Columns.Add("U");
            Dt.Columns.Add("QTE");
            Dt.Columns.Add("PU");
            Dt.Columns.Add("TOTAL");

            dgvUpdate.DataSource = Dt;
        }
        void resizeDGV()
        {
            dgvUpdate.RowHeadersWidth = 57;
            dgvUpdate.Columns[0].Width = 40;
            dgvUpdate.Columns[1].Width = 243;
            dgvUpdate.Columns[2].Width = 48;
            dgvUpdate.Columns[3].Width = 50;
            dgvUpdate.Columns[4].Width = 118;
            dgvUpdate.Columns[5].Width = 132;
        }

        private void UpdateDevis_Load(object sender, EventArgs e)
        {
            CreateDataTable();
            resizeDGV();

            if (ds.Tables["Produits"] != null)
                ds.Tables["Produits"].Clear();

            dapProduits = new SqlDataAdapter("select * from produits", cnx);
            dapProduits.Fill(ds, "Produits");
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (ds.Tables["UpdateDevis"] != null)
                    ds.Tables["UpdateDevis"].Clear();

                dap = new SqlDataAdapter("select NumProduits, Designations, Unite, Quantite, PrixUnitaire, Total from Produits where NumDevis = '" + txtSearch.Text + "'", cnx);
                dap.Fill(ds, "UpdateDevis");
                dgvUpdate.DataSource = ds.Tables["UpdateDevis"];

                for (int i = 0; i < dgvUpdate.Rows.Count ; i++)
                {
                    ligne = Dt.NewRow();
                    ligne[0] = dgvUpdate.Rows[i].Cells[0].Value.ToString();
                    ligne[1] = dgvUpdate.Rows[i].Cells[1].Value.ToString();
                    ligne[2] = dgvUpdate.Rows[i].Cells[2].Value.ToString();
                    ligne[3] = dgvUpdate.Rows[i].Cells[3].Value.ToString();
                    ligne[4] = dgvUpdate.Rows[i].Cells[4].Value.ToString();
                    ligne[5] = dgvUpdate.Rows[i].Cells[5].Value.ToString();

                    Dt.Rows.Add(ligne);
                }
                //MessageBox.Show(Dt.Rows.Count.ToString());
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
            
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void dgvUpdate_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                txtNum.Text = dgvUpdate.CurrentRow.Cells[0].Value.ToString();
                txtNameProducts.Text = dgvUpdate.CurrentRow.Cells[1].Value.ToString();
                txtUnit.Text = dgvUpdate.CurrentRow.Cells[2].Value.ToString();
                txtQuantity.Text = dgvUpdate.CurrentRow.Cells[3].Value.ToString();
                txtPrice.Text = dgvUpdate.CurrentRow.Cells[4].Value.ToString();
                txtTotal.Text = dgvUpdate.CurrentRow.Cells[5].Value.ToString();

                dgvUpdate.Rows.RemoveAt(dgvUpdate.CurrentRow.Index);
                txtQuantity.Focus();
            }
            catch
            {
                return;
            }
        }

        private void dgvUpdate_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            txtTotalTTC.Text = (from DataGridViewRow row in dgvUpdate.Rows
                                where row.Cells[5].FormattedValue.ToString() != string.Empty
                                select Convert.ToDouble(row.Cells[5].FormattedValue)).Sum().ToString();
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

        private void txtQuantity_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != 8)
                e.Handled = true;
        }

        private void txtQuantity_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && txtQuantity.Text != "" && txtPrice.Text != "" && txtNameProducts.Text != "" && txtUnit.Text != "" && txtTotal.Text != "")
            {
                for (int i = 0; i < dgvUpdate.Rows.Count - 1; i++)
                {
                    if (dgvUpdate.Rows[i].Cells[0].Value.ToString() == txtNum.Text)
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

                for (int i = 0; i < Dt.Rows.Count; i++)
                {
                    //MessageBox.Show(Dt.Rows[i][0].ToString());
                    if (txtNum.Text == Dt.Rows[i][0].ToString())
                        Dt.Rows.RemoveAt(i);
                }

                Dt.Rows.Add(r);
                dgvUpdate.DataSource = Dt;

                ClearBoxes();

                txtTotalTTC.Text = (from DataGridViewRow row in dgvUpdate.Rows
                                    where row.Cells[5].FormattedValue.ToString() != string.Empty
                                    select Convert.ToDouble(row.Cells[5].FormattedValue)).Sum().ToString();
            }
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            dgvUpdate.Rows.RemoveAt(dgvUpdate.CurrentRow.Index);
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

        DataRow lgn;
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                //Delete Old Products
                cmd.Connection = cnx;
                cmd.CommandText = "delete  from Produits where NumDevis ='" + txtSearch.Text + "'";
                cnx.Open();
                cmd.ExecuteNonQuery();
                cnx.Close();

                //Add all products in tables 

                for (int i = 0; i < dgvUpdate.Rows.Count ; i++)
                {
                    lgn = ds.Tables["Produits"].NewRow();
                    lgn[1] = dgvUpdate.Rows[i].Cells[1].Value.ToString();
                    lgn[2] = dgvUpdate.Rows[i].Cells[2].Value.ToString();
                    lgn[3] = dgvUpdate.Rows[i].Cells[3].Value.ToString();
                    lgn[4] = dgvUpdate.Rows[i].Cells[4].Value.ToString();
                    lgn[5] = dgvUpdate.Rows[i].Cells[5].Value.ToString();
                    lgn[6] = txtSearch.Text;

                    ds.Tables["Produits"].Rows.Add(lgn);

                }

                cb = new SqlCommandBuilder(dapProduits);
                dapProduits.Update(ds, "Produits");

                MessageBox.Show("Ajouter avec succes");
                ClearBoxes();

            }
            catch (Exception ex)
            {
                MessageBox.Show("ERREUR -- Details : " + ex.Message, "Erreur !!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                param.Value = txtSearch.Text;
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

        private void modifierToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dgvUpdate_DoubleClick(sender, e);
        }

        private void supprimerLigneSelectionneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dgvUpdate.Rows.RemoveAt(dgvUpdate.CurrentRow.Index);
        }

        private void supprimerToutLesLignesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dt.Rows.Clear();
            dgvUpdate.Refresh();
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
    }
}
