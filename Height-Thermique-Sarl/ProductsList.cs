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
    public partial class ProductsList : Form
    {
        public ProductsList()
        {
            InitializeComponent();
        }

        SqlConnection cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["connect"].ConnectionString);
        DataSet ds = new DataSet();
        SqlCommand cmd = new SqlCommand();
        SqlDataAdapter dap = new SqlDataAdapter();
        SqlDataAdapter dapSearch = new SqlDataAdapter();
        SqlCommandBuilder cb;

        private void ProductsList_Load(object sender, EventArgs e)
        {
            if (ds.Tables["Products"] != null)
                ds.Tables["Products"].Clear();
            
            dap = new SqlDataAdapter("select * from AllProducts", cnx);
            //MessageBox.Show(cnx.State.ToString());
            dap.Fill(ds, "Products");
            dgvProducts.DataSource = ds.Tables["Products"];

            
        }

        private void dgvProducts_DoubleClick(object sender, EventArgs e)
        {
            Close();
        }

        private void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            cmd = new SqlCommand("Search_Products", cnx);
            cmd.CommandType = CommandType.StoredProcedure;

            SqlParameter param = cmd.Parameters.Add("@wordSearch", SqlDbType.VarChar, 50);
            param.Value = txtSearch.Text;


            dapSearch = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            dapSearch.Fill(dt);

            dgvProducts.DataSource = dt;
        }
    }
}
