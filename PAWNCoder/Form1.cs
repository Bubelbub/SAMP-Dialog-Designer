using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

namespace PAWNCoder
{
    public partial class Form1 : Form
    {
        private const int EM_SETTABSTOPS = 0x00CB;

        [DllImport("User32.dll")]
        static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int[] lParam);
        bool check = false;
        PAWNCoder.Properties.Settings lastcoding = new PAWNCoder.Properties.Settings();
        public Form1()
        {
            Timer timer = new Timer();
            timer.Interval = 750;
            timer.Tick += new EventHandler(timerevent);
            timer.Start();
            InitializeComponent();
            try { richTextBox1.LoadFile("PAWNCoder.last", RichTextBoxStreamType.RichText); } catch { }
            label2.Text = richTextBox1.Text.Length.ToString() + " Zeichen";
            var pos = this.PointToScreen(label1.Location);
            pos = pictureBox2.PointToClient(pos);
            label1.Parent = pictureBox2;
            label1.Location = pos;
            label1.BackColor = Color.Transparent;

            pos = this.PointToScreen(pictureBox2.Location);
            pos = pictureBox1.PointToClient(pos);
            pictureBox2.Parent = pictureBox1;
            pictureBox2.Location = pos;
            pictureBox2.BackColor = Color.Transparent;

            this.richTextBox1.KeyDown += new KeyEventHandler(richTextBox1_KeyDown);
            this.textBox2.KeyDown += new KeyEventHandler(textBox2_KeyDown);
            int[] tabstops = new int[] { 23 };
            SendMessage(richTextBox1.Handle, EM_SETTABSTOPS, tabstops.Length, tabstops);
            richTextBox1.Invalidate();
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        }
        private void timerevent(Object e, EventArgs unused)
        {
            if (!check)
            {
                label1.Text = richTextBox1.Text;
                label1.Text = Regex.Replace(label1.Text,"\t", "             ");
                Size test = new Size(label1.Width + 30, label1.Height + 60);
                if (test.Width < 180) test.Width = 180;
                pictureBox2.Size = test;
                test.Height = 21;
                pictureBox3.Size = test;
                Point p = new Point(pictureBox1.Location.X + pictureBox2.Location.X + pictureBox2.Width/2-50,
                    pictureBox1.Location.Y + pictureBox2.Location.Y + pictureBox2.Height - 35);
                pictureBox4.Location = p;
                string[] lines = Regex.Split(label1.Text, "\r\n");
                lastcoding.Save();
            }
            check = false;
        }
        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            int index = label1.Text.IndexOf('\t');
            label2.Text = richTextBox1.Text.Length.ToString() + " Zeichen";
            try { richTextBox1.SaveFile("PAWNCoder.last", RichTextBoxStreamType.RichText); } catch { }
        }
        private void richTextBox1_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            check = true;
            if (e.Control && e.KeyValue == 65)
                richTextBox1.SelectAll();
        }
        private void textBox2_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.Control && e.KeyValue == 65)
                textBox2.SelectAll();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox1.Select(0, 0);

            string convertText = "";
            Color last = Color.FromName("Black");

            for (int j = 0; j < richTextBox1.TextLength; j++)
            {
                richTextBox1.Select(j, 1);
                Color k = richTextBox1.SelectionColor;
                string colorcode = "";
                if (k != last)
                {
                    colorcode = string.Format("0x{0:X8}", k.ToArgb());
                    colorcode = colorcode.Substring(colorcode.Length - 6, 6);
                    convertText += "{" + colorcode + "}";
                    last = k;
                }
                convertText += richTextBox1.SelectedText;
            }

            convertText = Regex.Replace(convertText, '\t'.ToString(), "\\t");
            convertText = Regex.Replace(convertText, '"'.ToString(), "\\\"");
            convertText = Regex.Replace(convertText, '%'.ToString(), "%%");

            string[] lines = Regex.Split(convertText, "\n");
            textBox2.Text = "new str[" + (convertText.Length + (lines.Length - 1)) + "];";
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Length < 1 && i != lines.Length - 1) lines[i + 1] = "\\n" + lines[i + 1];
                else if(lines[i].Length < 1 && i == lines.Length - 1) continue;
                else textBox2.Text += "\r\nformat(str, sizeof str, \"" + (i == 0 ? ("") : ("%s")) + "" + lines[i] + "" + ((i == lines.Length - 1) ? "" : "\\n") + "\", str);";
            }
            textBox2.Text += "\r\nShowPlayerDialog(playerid, dialogid, " + comboBox1.Text + ", \"Headline\", str, \"Ok!\", \"\");";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ColorDialog colorDlg = new ColorDialog();
            colorDlg.AllowFullOpen = true;
            colorDlg.FullOpen = true;
            colorDlg.AnyColor = true;
            colorDlg.SolidColorOnly = false;
            colorDlg.Color = label1.ForeColor;            
            if (colorDlg.ShowDialog() == DialogResult.OK)
            {
                richTextBox1.SelectionColor = colorDlg.Color;
            }
        }
    }
}
