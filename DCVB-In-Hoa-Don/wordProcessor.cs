using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Word = Microsoft.Office.Interop.Word;
using System.IO;
using System.Windows.Forms;

namespace DCVB_In_Hoa_Don
{
    internal class wordProcessor
    {
        private Word.Application wordApp;
        private Word.Document wordDoc;
        private String fileLocation;

        /////// Constructor, get, set.....
        public wordProcessor()
        {
            wordApp = new Word.Application();
        }
        public void setWordDocWithAddress(String fileAddress)
        {
            try
            {
                wordDoc = wordApp.Documents.Open(fileAddress);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /////// Save and quit word operations
        public void closeInstance()
        {
            wordDoc.Close(SaveChanges: false);
            wordDoc = null;
            wordApp.Quit(SaveChanges: false);
        }

        public void saveInstance(string Address)
        {
            wordDoc.SaveAs2(Address);
        }


        /////////
        /// Function name: insertCommas
        /// Function description: insert a comma on every 3rd zero
        ////////
        private string insertCommas(string str)
        {
            if (str.Length < 4)
            {
                return str;
            }
            //first sub string get the first under 3 length numbers
            return insertCommas(str.Substring(0, str.Length - 3)) + "," + insertCommas(str.Substring(str.Length - 3, 3));
        }

        //////////////
        /// Function name: findAndReplaceWith
        /// Function description: Filling the boilerplate bs of word
        //////////////
        private void findAndReplaceWith(Word.Application wordApp, object findText, object replaceWithText)
        {
            wordApp.Selection.Find.Execute(ref findText, true,
                                            true, false, false, false, false, 1, false,
                                             replaceWithText, 2, false, false, false, false);
        }

        //////////
        /// Function name: printWithDoc
        /// Function description: Print with a designated word document 
        //////////
        public void printWithDoc()
        {
            try
            {
                PrintDialog pDiag = new PrintDialog();
                if (pDiag.ShowDialog() == DialogResult.OK)
                {
                    this.wordApp.ActivePrinter = pDiag.PrinterSettings.PrinterName;
                    this.wordDoc.PrintOut();
                    // This sleep timer is to wait for ms to send the print command
                    // to the printer and ms doesn't have any method to wait for this
                    System.Threading.Thread.Sleep(4000);
                    this.wordApp.ActivePrinter = null;
                    this.closeInstance();
                    MessageBox.Show("Đã gửi thao tác In tới máy in!");
                }
                else
                {
                    this.closeInstance();
                }
            }
            catch (Exception error)
            {
                MessageBox.Show("Lỗi" + error.Message);
                this.closeInstance();
            }
        }

        //////////////
        /// Function name: printFromAddress
        /// Function Description: Print the template file from a designated address 
        /// using the word application then close said application
        //////////////
        public void printFromAddress(String fileAddress)
        {
            try
            {
                this.fileLocation = fileAddress;
                this.wordDoc = wordApp.Documents.Open(fileLocation);
                this.printWithDoc();
            }
            catch (Exception error)
            {
                MessageBox.Show("Lỗi: " + error.Message);
                this.closeInstance();
            }
        }

        //////////
        /// Function name: printTestFile
        /// Function description: Test print a file to see if
        /// the Program print the document
        //////////
        public void printTestFile()
        {
            this.printFromAddress(AppDomain.CurrentDomain.BaseDirectory + @"\documentTemplates\InWordK80.docx");
        }

        public void replaceWords(Dictionary<String, String> find_and_replace_dict)
        {
            try
            {
                foreach (var entry in find_and_replace_dict)
                {
                    String find = entry.Key;
                    String replace = entry.Value;
                    findAndReplaceWith(wordApp, find, replace);
                }
            }
            catch (Exception error)
            {
                MessageBox.Show("Lỗi: " + error.Message);
            }
        }

        public void customFillingTable(DataGridView grid)
        {
            try
            {
                int colIndex = 0;
                int rowIndex = 0;
                Word.Table table = wordDoc.Tables[1];
                grid.AllowUserToAddRows = false;

                foreach (DataGridViewRow row in grid.Rows)
                {
                    if (rowIndex < grid.Rows.Count - 1)
                    {
                        table.Rows.Add(table.Cell(rowIndex + 10, 1));
                    }
                    colIndex = 0;
                    foreach (DataGridViewColumn col in grid.Columns)
                    {
                        var gridCellValue = grid[colIndex, rowIndex].Value;
                        if (gridCellValue != null)
                        {
                            string str = gridCellValue.ToString();
                            if (colIndex == 4)
                            {
                                str = insertCommas(str);
                            }
                            table.Cell(rowIndex + 10, colIndex + 1).Range.Text = str;
                        }
                        else
                        {
                            table.Cell(rowIndex + 10, colIndex + 1).Range.Text = "";
                        }
                        colIndex++;
                    }
                    rowIndex++;
                }
            }
            catch (Exception error)
            {
                MessageBox.Show("Lỗi : " + error.Message);
            }
        }
    }
}
