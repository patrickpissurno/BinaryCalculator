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
        public Form1()
        {
            InitializeComponent();
            radioButton3.Select();
        }

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
    }
}
