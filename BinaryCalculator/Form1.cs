﻿using System;
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

        string value0 = "0";
        string value1 = "0";
        string op = "";

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
            ProcessData();
            op = "";
        }

        #region ProcessData
        void ProcessData()
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
                textBox1.Text = "0";
            UpdateValues(textBox1.Text);
            if (Normalize(value0, "0")[0] != Normalize(value0, "0")[1] || Normalize(value1, "0")[0] != Normalize(value1, "0")[1])
            {
                switch (op)
                {
                    case "+":
                        SetText(Sum(value1, value0));
                        break;
                    case "*":
                        SetText(Plus(value1, value0));
                        break;
                    case "-":
                        SetText(Subtract(value1, value0));
                        break;
                    case "/":
                        SetText(Division(value1, value0));
                        break;
                }
            }
        }
        #endregion

        #region OP Buttons
        private void op_Click(object sender, EventArgs e)
        {
            if (op != "")
                ProcessData();
            UpdateValues(textBox1.Text);
            textBox1.Text = "";
            op = (sender as Button).Text;
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
            value1 = value0;
            value0 = TrimZeros(val);
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
    }
}
