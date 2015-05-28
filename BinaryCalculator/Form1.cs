using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BinaryCalculator
{
    public partial class Form1 : Form
    {
        public string lastText = "";

        int value0 = 0;
        int value1 = 0;

        public Form1()
        {
            InitializeComponent();
            radioButton3.Select();
        }

        #region Input Handling
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(textBox1.Text))
                {
                    float.Parse(textBox1.Text);
                    if (textBox1.Text.IndexOfAny(new char[] { '2', '3', '4', '5', '6', '7', '8', '9', 'E', 'e', '.', ',' }) == -1)
                        lastText = textBox1.Text.Trim();
                    else
                        throw new Exception();
                }
            }
            catch
            {
                int lastPos = textBox1.SelectionStart;
                textBox1.Text = lastText;
                try
                {
                    textBox1.SelectionStart = lastPos - 1;
                }
                catch { }
            }

        }
        #endregion

        private void button7_Click(object sender, EventArgs e)
        {
            //MessageBox.Show(Division("100","10"));
            //MessageBox.Show(Subtract("100", "10"));
            MessageBox.Show(Sum(Normalize("101", "1")[0], Normalize("101", "1")[1]));
        }

        #region Sum
        string Sum(string val0, string val1)
        {
            val0 = Normalize(val0, val1)[0];
            val1 = Normalize(val0, val1)[1];

            string result = "";
            string next_bonus = "0";
            for (int i = val0.Length; i > 0; i--)
            {
                if ((val0[i - 1] == '1' && val1[i - 1] == '1') || ((val0[i - 1] == '1' || val1[i - 1] == '1') && next_bonus == "1"))
                {
                    if (((val0[i - 1] == '1' && val1[i - 1] == '1') && next_bonus == "0") || ((val0[i - 1] == '1' && val1[i - 1] == '0') && next_bonus == "1") || ((val0[i - 1] == '0' && val1[i - 1] == '1') && next_bonus == "1"))
                    {
                        result = "0" + result;
                        next_bonus = "1";
                    }
                    else if ((val0[i - 1] == '1' && val1[i - 1] == '1') && next_bonus == "1")
                    {
                        result = "1" + result;
                        next_bonus = "1";
                    }
                    if (i == 1)
                        result = next_bonus + result;
                }
                else if (val0[i - 1] == '0' && val1[i - 1] == '0')
                    result = "0" + result;
                else
                    result = "1" + result;
            }
            return result;
        }
        #endregion

        #region Plus
        string Plus(string val, string total)
        {
            string result = "0";
            for (string i = "0"; i != total; i = Sum(i, "1"))
                result = Sum(result, val);
            return result;
        }
        #endregion

        #region Subtract
        string Subtract(string val0, string val1)
        {
            val0 = Normalize(val0, val1)[0];
            val1 = Normalize(val0, val1)[1];

            string result = "";
            val1 = val1.Replace('0', 'z');
            val1 = val1.Replace('1', '0');
            val1 = val1.Replace('z', '1');
            
            val1 = Sum(val1, "1");

            result = Sum(val0, val1);
            MessageBox.Show(val0 + "\n" + val1 + "\n" + result);

            result = result.Substring(1, result.Length - 1);
            return result;
        }

        string[] Normalize(string val0, string val1)
        {
            while (val0.Length > val1.Length)
                val1 = "0" + val1;
            while (val1.Length > val0.Length)
                val0 = "0" + val0;
            return new string[] { val0, val1 };
        }
        #endregion

        string Division(string val0, string val1)
        {
            val0 = Normalize(val0, val1)[0];
            val1 = Normalize(val0, val1)[1];

            string result = "0";
            while (true)
            {
                if (val0 != Normalize(val0, "0")[1])
                {
                    val0 = Subtract(val0, val1);
                    MessageBox.Show(Subtract("100", "010"));
                    result = Sum(result, "1");
                }
                else
                    break;
                MessageBox.Show(val0 + "\n" + Normalize(val0, "0")[1] + "\n" + result);
            }
            return result;
        }
    }
}
