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

        const int MAX_LENGTH = 30;

        string value0 = "0";
        string value1 = "0";
        string op = "";

        bool ignoreBlock = false;

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
                    if (textBox1.Text.Length > MAX_LENGTH)
                        throw new Exception();
                    if (!ignoreBlock)
                    {
                        float.Parse(textBox1.Text);
                        if (textBox1.Text.IndexOfAny(new char[] { '2', '3', '4', '5', '6', '7', '8', '9', 'E', 'e', '.', ',' }) == -1)
                            lastText = textBox1.Text.Trim();
                        else
                            throw new Exception();
                    }
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

        #region Equals Button
        private void button7_Click(object sender, EventArgs e)
        {
            ProcessData();
            op = "";
        }
        #endregion

        #region ProcessData
        void ProcessData()
        {            
            if (string.IsNullOrWhiteSpace(lastText))
                textBox1.Text = "0";
            UpdateValues(lastText);

            if (Normalize(value0, "0")[0] != Normalize(value0, "0")[1] || Normalize(value1, "0")[0] != Normalize(value1, "0")[1])
            {
                switch (op)
                {
                    case "+":
                        SetText(ChangeOutput(Sum(value1, value0)));
                        break;
                    case "*":
                        SetText(ChangeOutput(Plus(value1, value0)));
                        break;
                    case "-":
                        SetText(ChangeOutput(Subtract(value1, value0)));
                        break;
                    case "/":
                        SetText(ChangeOutput(Division(value1, value0)));
                        break;
                    case "":
                        SetText(ChangeOutput(value0));
                        break;
                }
            }
        }
        #endregion

        #region ChangeOutput
        string ChangeOutput(string val)
        {
            if (radioButton3.Checked)
                return TrimZeros(val);
            else if (radioButton2.Checked)
            {
                ignoreBlock = true;
                return BinaryToDecimal(val);
            }
            else if (radioButton1.Checked)
            {
                ignoreBlock = true;
                return BinaryToHexadecimal(val);
            }
            return val;
        }
        #endregion

        #region OP Buttons
        private void op_Click(object sender, EventArgs e)
        {
            ignoreBlock = false;
            if (op != "")
                ProcessData();
            UpdateValues(textBox1.Text);
            textBox1.Text = "";
            op = (sender as Button).Text;
        }
        #endregion

        #region Num Buttons
        private void num_Click(object sender, EventArgs e)
        {
            ignoreBlock = false;
            textBox1.AppendText((sender as Button).Text);
        }
        #endregion

        #region Util
        void SetText(string val)
        {
            UpdateValues(val);
            textBox1.Text = TrimZeros(val);
        }

        void UpdateValues(string val)
        {
            if (!ignoreBlock && radioButton3.Checked)
            {
                value1 = value0;
                value0 = TrimZeros(val);
            }
        }
        #endregion

        #region Sum
        string Sum(string val0, string val1)
        {
            val0 = Normalize(val0, val1)[0];
            val1 = Normalize(val0, val1)[1];

            string result = "";
            string next_bonus = "0";
            for (int i = val0.Length; i > 0; i--)
            {
                int n = i - 1;
                if (next_bonus == "1")
                {
                    if (val0[n] != val1[n])
                    {
                        result = "0" + result;
                        next_bonus = "1";
                    }
                    else
                    {
                        if (val0[n] == '0')
                            next_bonus = "0";
                        else
                            next_bonus = "1";
                        result = "1" + result;
                    }
                }
                else
                {
                    if (val0[n] != val1[n])
                    {
                        result = "1" + result;
                        next_bonus = "0";
                    }
                    else
                    {
                        result = "0" + result;
                        if (val0[n] == '0')
                            next_bonus = "0";
                        else
                            next_bonus = "1";
                    }
                }
            }
            if (next_bonus == "1")
                result = "1" + result;
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
            result = result.Substring(1, result.Length - 1);
            return TrimZeros(result);
        }
        #endregion

        #region Division
        string Division(string val0, string val1)
        {
            val0 = Normalize(val0, val1)[0];
            val1 = Normalize(val0, val1)[1];
            string result = "0";
            while (true)
            {
                if (IsGreater(val0, "0"))
                {
                    val0 = Subtract(val0, val1);
                    result = Sum(result, "1");
                }
                else
                    break;
                if (IsGreater(val1, val0))
                    break;
            }
            return result;
        }
        #endregion

        #region TrimZeros
        string TrimZeros(string val)
        {
            if (val.Length > 1)
            {
                bool found = false;
                for (int i = 0; i < val.Length; i++)
                {
                    if(!found)
                    {
                        if (val[i] != '0')
                            found = true;
                    }
                    if (found)
                        return val.Substring(i, val.Length - i);
                }
                if (Normalize(val, "0")[0] == Normalize(val, "0")[1])
                    return "0";
                return val;
            }
            else
                return val;
        }
        #endregion

        #region Normalize
        string[] Normalize(string val0, string val1)
        {
            while (val0.Length > val1.Length)
                val1 = "0" + val1;
            while (val1.Length > val0.Length)
                val0 = "0" + val0;
            return new string[] { val0, val1 };
        }
        #endregion

        #region IsGreater
        bool IsGreater(string val0, string val1)
        {
            val0 = Normalize(val0, val1)[0];
            val1 = Normalize(val0, val1)[1];

            for (int i = 0; i < val0.Length; i++)
            {
                if(val1[i] == '1' && val0[i] == '0')
                    break;
                if (val0[i] == '1' && val1[i] == '0')
                    return true;
            }
            return false;
        }
        #endregion

        #region BinaryToDecimal
        string BinaryToDecimal(string val)
        {
            val = TrimZeros(val);
            List<int> sum = new List<int>();

            //Fill the list
            for (int i = val.Length; i > 0; i--)
                sum.Add((int)Math.Pow(2,i-1));

            int result = 0;
            for (int i = 0; i < val.Length; i++)
            {
                if (val[i] == '1')
                    result += sum[i];
            }

            return result.ToString();
        }
        #endregion

        #region BinaryToHexadecimal
        string BinaryToHexadecimal(string val)
        {
            val = AddZeros(val);
            string result = "";
            for (int i = 0; i < val.Length / 4; i++)
            {
                switch (val.Substring(i * 4, 4))
                {
                    case "0000":
                        result += "0";
                        break;
                    case "0001":
                        result += "1";
                        break;
                    case "0010":
                        result += "2";
                        break;
                    case "0011":
                        result += "3";
                        break;
                    case "0100":
                        result += "4";
                        break;
                    case "0101":
                        result += "5";
                        break;
                    case "0110":
                        result += "6";
                        break;
                    case "0111":
                        result += "7";
                        break;
                    case "1000":
                        result += "8";
                        break;
                    case "1001":
                        result += "9";
                        break;
                    case "1010":
                        result += "A";
                        break;
                    case "1011":
                        result += "B";
                        break;
                    case "1100":
                        result += "C";
                        break;
                    case "1101":
                        result += "D";
                        break;
                    case "1110":
                        result += "E";
                        break;
                    case "1111":
                        result += "F";
                        break;
                }
            }
            return result;
        }
        #endregion

        #region AddZeros
        string AddZeros(string val)
        {
            string result = val;
            if (val.Length < 4)
            {
                for (int i = 0; i < 4 - val.Length; i++)
                    result = "0" + result;
            }
            else if (val.Length % 4 != 0)
            {
                for (int i = 0; i < val.Length % 4; i++)
                    result = "0" + result;
            }
            return result;
        }
        #endregion

        #region Events
        private void textBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (ignoreBlock)
            {
                ignoreBlock = false;
                textBox1.Text += "|";
            }
        }

        private void output_Selected(object sender, EventArgs e)
        {
            textBox1.Text = ChangeOutput(lastText);
        }
        #endregion
    }
}
