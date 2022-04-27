using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace DCVB_In_Hoa_Don
{
    public partial class printNote : Form
    {
        private String date;
        private String note;
        public printNote()
        {
            InitializeComponent();
        }

        private void printNote_Load(object sender, EventArgs e)
        {
            this.date = date_lbl.Text = System.DateTime.Now.Date.ToString("dd/MM/yyyy");
            file_name_lbl.Text = "Không có file";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Word 2015 Documents File (*.docx)|*.docx";
            saveFileDialog1.FileName = "Ghi-chu.docx";
            saveFileDialog1.FilterIndex = 0;
            saveFileDialog1.InitialDirectory = @"%USERPROFILE%\Dekstop";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.note = note_txtbox.Text;
                Dictionary<String, String> find_and_replace_list = new Dictionary<String, String>();
                find_and_replace_list.Add("{billedDate}", this.date);
                find_and_replace_list.Add("{note}", this.note);

                wordProcessor wordProc = new wordProcessor();
                wordProc.setWordDocWithAddress(AppDomain.CurrentDomain.BaseDirectory + @"\documentTemplates\Ghi-chu-Mau.docx");
                wordProc.replaceWords(find_and_replace_list);
                wordProc.saveInstance(saveFileDialog1.FileName);
                wordProc.printWithDoc();
            }
            else
            {
                MessageBox.Show("Đã huỷ thao tác lưu và ngừng in.");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            dialogue_openfiledialog.Filter = "PDF documents (*.pdf)|*.pdf|Text document (*.txt)|*.txt";
            dialogue_openfiledialog.FilterIndex = 0;
            dialogue_openfiledialog.InitialDirectory = @"%USERPROFILE%\Dekstop";
            if (dialogue_openfiledialog.ShowDialog() == DialogResult.OK)
            {
                file_name_lbl.Text = System.IO.Path.GetFileName(dialogue_openfiledialog.FileName);
                string[] fileType = dialogue_openfiledialog.FileName.Split('.');
                string textBoxString = "";
                switch (fileType[1])
                {
                    case "txt":
                        string[] lines = System.IO.File.ReadAllLines(dialogue_openfiledialog.FileName);
                        foreach (string line in lines)
                        {
                            textBoxString += line + Environment.NewLine;
                        }
                        note_txtbox.Text = textBoxString;
                        break;
                    case "pdf":
                        Console.WriteLine("pdf file opened");
                        PdfDocument pdfDoc = PdfDocument.Open(dialogue_openfiledialog.FileName);
                        foreach (Page page in pdfDoc.GetPages())
                        {
                            IEnumerable<Word> words = page.GetWords();
                            foreach (Word word in words)
                            {
                                textBoxString += word.ToString() + " ";
                            }
                        }
                        note_txtbox.Text = textBoxString;
                        break;
                    default:
                        throw new Exception();

                }
            }
        }
    }
}
