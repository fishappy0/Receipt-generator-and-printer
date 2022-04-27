using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Word = Microsoft.Office.Interop.Word;

namespace DCVB_In_Hoa_Don
{
    public partial class mainForm : Form
    {
        public mainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            wordProcessor printDocument = new wordProcessor();
            printDocument.printTestFile();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            printLabel label_form = new printLabel();
            label_form.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            printNote note_form = new printNote();
            note_form.ShowDialog();
        }

        private void mainForm_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
        }
    }
}
