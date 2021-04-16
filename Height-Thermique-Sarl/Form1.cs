using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// This is the code for your desktop app.
// Press Ctrl+F5 (or go to Debug > Start Without Debugging) to run your app.

namespace Height_Thermique_Sarl
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void devisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form devis = new Devis();
            devis.MdiParent = this;
            if (this.ActiveMdiChild != null)
                ActiveMdiChild.Close();
            devis.Show();
        }

        private void factureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form devis = new Frm_Facture();
            devis.MdiParent = this;
            if (this.ActiveMdiChild != null)
                ActiveMdiChild.Close();
            devis.Show();
        }

        private void devisToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Form devis = new Archive_Devis();
            devis.MdiParent = this;
            if (this.ActiveMdiChild != null)
                ActiveMdiChild.Close();
            devis.Show();
        }

        private void factureToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Form devis = new Archive_Facuture();
            devis.MdiParent = this;
            if (this.ActiveMdiChild != null)
                ActiveMdiChild.Close();
            devis.Show();
        }

        private void devisUpdateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form devis = new UpdateDevis();
            devis.MdiParent = this;
            if (this.ActiveMdiChild != null)
                ActiveMdiChild.Close();
            devis.Show();
        }

        private void factureUpdateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form devis = new UpdateFacture();
            devis.MdiParent = this;
            if (this.ActiveMdiChild != null)
                ActiveMdiChild.Close();
            devis.Show();
        }

        private void situationToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            Form frm = new frm_STT();
            frm.MdiParent = this;
            if (this.ActiveMdiChild != null)
                ActiveMdiChild.Close();
            frm.Show();
        }

        private void situationToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Form frm = new Archive_Situation();
            frm.MdiParent = this;
            if (this.ActiveMdiChild != null)
                ActiveMdiChild.Close();
            frm.Show();
        }

        private void situationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form frm = new Update_Situation();
            frm.MdiParent = this;
            if (this.ActiveMdiChild != null)
                ActiveMdiChild.Close();
            frm.Show();
        }
    }
}
