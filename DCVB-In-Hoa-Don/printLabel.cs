using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Word = Microsoft.Office.Interop.Word;
using DCVB_In_Hoa_Don.documentTemplates;

namespace DCVB_In_Hoa_Don
{
    public partial class printLabel : Form
    {
        private String date;
        private String customerName;
        private String customerAddress;
        private String customerPhone;
        private String sellerName;
        private String totalQuantity;
        private String totalPrice;
        private String totalPriceText;
        private String note;

        public printLabel()
        {
            InitializeComponent();
        }

        ///////// 
        /// Function Name: moneyToString
        /// Function Description: converting money number to its "in words" text form
        /// example: 1,000,000 -> Mot trieu dong chan
        /// Dev comment: Below function is verbose and gross, needs clean up
        ///////// 
        private String moneyToString(float money)
        {
            // Declaring the numbers names
            string[] unitNumbers = new string[] { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
            string[] placeValues = new string[] { "", "nghìn", "triệu", "tỷ" };
            bool isNegative = false;

            // "#" Symbol to remove the "E" - exponential notation
            //     as well as decimals
            string sNumber = money.ToString("#");
            double number = Convert.ToDouble(sNumber);
            if (number < 0)
            {
                number = -number;
                sNumber = number.ToString();
                isNegative = true;
            }


            int ones, tens, hundreds;
            int positionDigit = sNumber.Length;   // last -> first
            string result = " ";


            if (positionDigit == 0)
                result = unitNumbers[0] + result;
            else
            {
                // 0:       ###
                // 1: nghìn ###,###
                // 2: triệu ###,###,###
                // 3: tỷ    ###,###,###,###
                int placeValue = 0;

                while (positionDigit > 0)
                {
                    // Check last 3 digits remain ### (hundreds tens ones)
                    tens = hundreds = -1;


                    // Warning 1: No type or string sanitation, this code below potentially
                    // causes issues when it cannot parse anything that is not number
                    ones = Convert.ToInt32(sNumber.Substring(positionDigit - 1, 1));
                    positionDigit--;
                    if (positionDigit > 0)
                    {
                        // Refer to Warning 1
                        tens = Convert.ToInt32(sNumber.Substring(positionDigit - 1, 1));
                        positionDigit--;
                        if (positionDigit > 0)
                        {
                            try
                            {
                                // Refer to Warning 1
                                hundreds = Convert.ToInt32(sNumber.Substring(positionDigit - 1, 1));
                                positionDigit--;
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                        }
                    }

                    if ((ones > 0) || (tens > 0) || (hundreds > 0) || (placeValue == 3))
                    {
                        result = placeValues[placeValue] + result;
                    }

                    placeValue++;
                    if (placeValue > 3) placeValue = 1;

                    if ((ones == 1) && (tens > 1))
                        result = "một " + result;
                    else
                    {
                        if ((ones == 5) && (tens > 0))
                            result = "lăm " + result;
                        else if (ones > 0)
                            result = unitNumbers[ones] + " " + result;
                    }
                    if (tens < 0)
                        break;
                    else
                    {
                        if ((tens == 0) && (ones > 0)) result = "lẻ " + result;
                        if (tens == 1) result = "mười " + result;
                        if (tens > 1) result = unitNumbers[tens] + " mươi " + result;
                    }
                    if (hundreds < 0) break;
                    else
                    {
                        if ((hundreds > 0) || (tens > 0) || (ones > 0))
                            result = unitNumbers[hundreds] + " trăm " + result;
                    }
                    result = " " + result;
                }
            }
            double decimalNumbers = money - Math.Floor(money);
            result = result.Trim();
            if (isNegative) result = "Âm " + result;
            return result + (decimalNumbers != 0 ? "" : " đồng chẵn");
        }
        private void updateTotal()
        {
            float money_amount = 0;
            float unit_price = 0;
            int quantity = 0;
            foreach (DataGridViewRow row in item_datagridview.Rows)
            {
                var valueOfQuantityCell = row.Cells[1].Value;
                var valueOfUnitPriceCell = row.Cells[3].Value;
                var valueOfMoneyCell = row.Cells[4].Value;

                // Total price Type Check
                if (valueOfMoneyCell != null && float.TryParse(valueOfMoneyCell.ToString(), out _))
                {
                    money_amount += float.Parse(valueOfMoneyCell.ToString());
                }
                else
                {
                    row.Cells[4].Value = "0";
                }

                // Unit Price Type check
                if (valueOfUnitPriceCell != null && int.TryParse(valueOfUnitPriceCell.ToString(), out _))
                {
                    unit_price += int.Parse(valueOfUnitPriceCell.ToString());
                }
                else
                {
                    row.Cells[3].Value = "0";
                }

                // Quantity Type Check
                if (valueOfQuantityCell != null && int.TryParse(valueOfQuantityCell.ToString(), out _))
                {
                    quantity += int.Parse(valueOfQuantityCell.ToString());
                }
                else
                {
                    row.Cells[1].Value = "1";
                }

            }
            this.totalPrice = money_lbl.Text = money_amount.ToString("N0") + " VND";
            this.totalPriceText = money_txt_lbl.Text = money_amount != 0 ? moneyToString(money_amount) : "Không đồng";
            this.totalQuantity = quantity_lbl.Text = (quantity - 1).ToString();

        }

        //////// Form Load:
        private void printLabel_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            date_lbl.Text = System.DateTime.Now.Date.ToString("dd/MM/yyyy");
            quantity_lbl.Text = "0";
            money_lbl.Text = "0" + " VND";
            money_txt_lbl.Text = "Không Đồng";
            item_datagridview.Columns[4].DefaultCellStyle.Format = "N2";

            //money_txt_lbl.Text = moneyToString(69420912);
        }

        private void item_datagridview_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            this.updateTotal();
        }
        private void item_datagridview_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            this.updateTotal();
        }
        private void item_datagridview_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            this.updateTotal();
        }

        private void findAndReplaceText(wordProcessor wordProc)
        {
            try
            {
                this.customerName = customer_name_txtbox.Text.Trim();
                this.customerAddress = customer_address_txtbox.Text.Trim();
                this.customerPhone = customer_phone_txtbox.Text.Trim();
                this.sellerName = seller_name_txtbox.Text.Trim();

                this.note = note_txtbox.Text.Trim();
                this.date = date_lbl.Text.Trim();

                string[] findList = { "{billedDate}", "{billedCustomer}", "{billedCustomerPhone}", "{billedCustomerAddress}", "{merchantName}", "{quantity}", "{totalPrice}", "{totalPriceText}", "{noteText}" };
                string[] replaceList = { this.date, this.customerName, this.customerPhone, this.customerAddress, this.sellerName, this.totalQuantity, this.totalPrice, this.totalPriceText, this.note };

                wordProc.setWordDocWithAddress(AppDomain.CurrentDomain.BaseDirectory + @"\documentTemplates\Hoa-don-Mau.docx");
                wordProc.replaceWordsWithArr(findList, replaceList);
                wordProc.customFillingTable(item_datagridview);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void openSaveDialog(Action<String> func)
        {
            saveFileDialog1.Filter = "Word 2015 Documents File (*.docx)|*.docx";
            saveFileDialog1.FileName = "Hoa-Don.docx";
            saveFileDialog1.FilterIndex = 0;
            saveFileDialog1.InitialDirectory = @"%USERPROFILE%\Dekstop";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                func(saveFileDialog1.FileName);
            }
            else
            {
                MessageBox.Show("Đã huỷ thao tác lưu và ngừng in.");
            }
        }

        //Save and Print
        private void button1_Click(object sender, EventArgs e)
        {
            openSaveDialog(fileLocation =>
           {
               wordProcessor wordProc = new wordProcessor();
               findAndReplaceText(wordProc);
               wordProc.saveInstance(fileLocation);
               wordProc.printWithDoc();
               item_datagridview.AllowUserToAddRows = true;
               wordProc = null;
           });
        }

        //Print without save
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                wordProcessor wordProc = new wordProcessor();
                findAndReplaceText(wordProc);
                wordProc.printWithDoc();
                item_datagridview.AllowUserToAddRows = true;
                wordProc = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //Save without print
        private void button2_Click(object sender, EventArgs e)
        {

            openSaveDialog(fileLocation =>
            {
                wordProcessor wordProc = new wordProcessor();
                findAndReplaceText(wordProc);
                item_datagridview.AllowUserToAddRows = true;
                wordProc.saveInstance(fileLocation);
                wordProc.closeInstance();
                wordProc = null;
            });
        }
    }
}
