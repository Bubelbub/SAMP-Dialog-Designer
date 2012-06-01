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
        public Form1()
        {
            Timer timer = new Timer();
            timer.Interval = 750;
            timer.Tick += new EventHandler(timerevent);
            timer.Start();
            InitializeComponent();
            textBox1.Text = "Dein Dialogcode hier";
            label2.Text = textBox1.Text.Length.ToString() + " Zeichen";
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

            this.textBox1.KeyDown += new KeyEventHandler(textBox1_KeyDown);
            this.textBox2.KeyDown += new KeyEventHandler(textBox2_KeyDown);
            int[] tabstops = new int[] { 26 };
            SendMessage(textBox1.Handle, EM_SETTABSTOPS, tabstops.Length, tabstops);
            textBox1.Invalidate();
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            
        }
        private void timerevent(Object e, EventArgs unused)
        {
            if (!check)
            {
                label1.Text = textBox1.Text;
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
            }
            check = false;
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            //label1.Text = textBox1.Text;
            int index = label1.Text.IndexOf('\t');
            label2.Text = textBox1.Text.Length.ToString() + " Zeichen";
        }
        private void textBox1_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            check = true;
            if (e.Control && e.KeyValue == 65)
                textBox1.SelectAll();
            /*if (e.KeyValue == 9)
            {
                
                string[] lines = Regex.Split(label1.Text, "\r\n");
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Length < 1) continue;
                    for (int u = 0; u < lines[i].Length; u++)
                    {
                        if (lines[i][u] == '\t')
                        {
                            label2.Text = lines[i].Remove(u, 0);
                            string blubb = "";
                            for (int a = u; a < lines[i].Length; a++)
                            {
                                blubb = blubb + " ";
                            }
                            lines[i] = lines[i].Insert(u, blubb);
                        }
                    }
                }
                //label1.Text = label1.Text.Replace('\t'.ToString(),blubb );
            }*/
        }
        private void textBox2_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.Control && e.KeyValue == 65)
                textBox2.SelectAll();
        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //string blubb = textBox1.Text.Replace('\t'.ToString(), ";");
            string[] lines = Regex.Split(textBox1.Text, "\r\n");
            lines[0] = lines[0].Replace('\t'.ToString(), "\\t");
            lines[0] = Regex.Replace(lines[0], '"'.ToString(), "\\\"");
            lines[0] = Regex.Replace(lines[0], '"'.ToString(), "\\\"");
            lines[0] = Regex.Replace(lines[0], '%'.ToString(), "%%");
            textBox2.Text = "new str[" + textBox1.Text.Length + "];\r\nformat(str,sizeof str,\"" + lines[0] + ""+ ( ( lines.Length > 1 ) ? "\\n" : "" )+"\");";
            for (int i = 1; i < lines.Length; i++)
            {
                lines[i] = lines[i].Replace('\t'.ToString(), "\\t");
                lines[i] = Regex.Replace(lines[i], '"'.ToString(), "\\\"");
                lines[i] = Regex.Replace(lines[i], '%'.ToString(), "%%");
                if (lines[i].Length < 1 && i != lines.Length -1) lines[i+1] = "\\n"+lines[i+1];
                else if(lines[i].Length < 1 && i == lines.Length - 1) continue;
                else
                    textBox2.Text = textBox2.Text + "\r\nformat(str,sizeof str,\"%s" + lines[i] + "" + ((i == lines.Length - 1) ? "" : "\\n") + "\",str);";
            }
            textBox2.Text = textBox2.Text + "\r\nShowPlayerDialog(playerid,dialogid,"+ comboBox1.Text +",\"Headline\",str,\"Ok!\",\"\");";

            


        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {

        }
    }
}
