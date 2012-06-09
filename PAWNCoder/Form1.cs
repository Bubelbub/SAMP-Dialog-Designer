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
using System.IO;
using System.Windows.Media;

namespace PAWNCoder
{
    public partial class Form1 : Form
    {
        private const int EM_SETTABSTOPS = 0x00CB;

        [DllImport("User32.dll")]
        static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int[] lParam);
        bool check = false;
        string savepath = "";
        PAWNCoder.Properties.Settings lastcoding = new PAWNCoder.Properties.Settings();
        public Form1()
        {
            InitializeComponent();

            saveFileDialog1.Filter = "SAMP PAWN Format|*.pwn";
            saveFileDialog1.Title = "Code speichern";

            Timer timer = new Timer();
            timer.Interval = 750;
            timer.Tick += new EventHandler(timerevent);
            timer.Start();

            // Laden
            richTextBox1.Rtf = Properties.Settings.Default.lastcode;
            richTextBox2.Text = Properties.Settings.Default.title;
            richTextBox3.Text = Properties.Settings.Default.button1;
            richTextBox4.Text = Properties.Settings.Default.button2;

            label4.Text = "";
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
            //this.KeyDown += new KeyEventHandler(Form1_KeyDown);

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
            Properties.Settings.Default.lastcode = richTextBox1.Rtf.ToString();
            Properties.Settings.Default.Save();
            label4.Text = "";
        }

        private void richTextBox1_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            check = true;
            if (e.Control && e.KeyValue == 65)
                richTextBox1.SelectAll();
            else if (e.Control && e.KeyValue == 83)
                speichernToolStripMenuItem_Click(new object[] { }, new EventArgs { });
        }

        private void textBox2_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.Control && e.KeyValue == 65)
                textBox2.SelectAll();
            else if (e.Control && e.KeyValue == 83)
                speichernToolStripMenuItem_Click(new object[] { }, new EventArgs { });
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
            textBox2.Text += "\r\nShowPlayerDialog(playerid, dialogid, " + comboBox1.Text + ", \"" + richTextBox2.Text + "\", str, \"" + richTextBox3.Text + "\", \"" + richTextBox4.Text + "\");";
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

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            Form AboutBox = new AboutBox1();
            AboutBox.Show();
            AboutBox.Focus();
        }

        private void button3_Click(object sender, EventArgs e) // Code degenerieren
        {
            bool ok = false;
            if (richTextBox1.Text.Length > 0)
            {
                DialogResult Rewrite = MessageBox.Show("Wollen Sie wirklich den obigen Code überschreiben?",
                      "Warnung",
                      MessageBoxButtons.YesNoCancel,
                      MessageBoxIcon.Question,
                      MessageBoxDefaultButton.Button2);

                if (Rewrite == DialogResult.Yes)
                    ok = true;
            }
            if(ok || richTextBox1.Text.Length == 0)
            {
                richTextBox1.Text = "";
                string text = textBox2.Text;
                text = Regex.Replace(text, "%s", "");
                int counter;
                int co = 0;
                CaptureCollection cc;
                GroupCollection gc;
                Regex r = new Regex("format(.*)\"(.*)\"");
                MatchCollection mc = r.Matches(text);
                string RdyText = "";
                foreach(Match m in mc)
                {
                    gc = m.Groups;
                    co += 1;
                    try
                    {
                        cc = gc[2].Captures;
                        counter = cc.Count;
                        for (int ii = 0; ii < counter; ii++)
                        {
                            string nTxt = cc[ii].Value;
                            nTxt = nTxt.Replace("\\n", "\n");
                            RdyText += nTxt /* + (co != mc.Count ? "\n" : "") */;
                        }
                    }
                    catch{}
                }
                richTextBox1.Text = RdyText;

                // Farben System - Umständlich by Bubelbub :D

                richTextBox1.Text = Regex.Replace(richTextBox1.Text, "{[0-9abcdefABCDEF]+}", ""); // Farbcodes entfernen
                int started = 0;
                int howmany = 0;
                string color = "";
                Font fnt = richTextBox1.Font;
                for (int x = 0; x < RdyText.Length; x++)
                {
                    if (RdyText[x] == '{')
                    {
                        started = 1;
                    }
                    else if (RdyText[x] == '}')
                    {
                        howmany++;
                        Color colorY = System.Drawing.ColorTranslator.FromHtml("#" + color);
                        richTextBox1.SelectionStart = (x + 1) - (howmany * 8);
                        richTextBox1.SelectionLength = richTextBox1.Text.Length;
                        richTextBox1.SelectionFont = fnt;
                        richTextBox1.SelectionColor = colorY;
                        color = "";
                        started = 0;
                    }
                    else if (started == 1)
                    {
                        color += RdyText[x];
                    }
                }
                richTextBox1.SelectionStart = -1;
            }
        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.title = richTextBox2.Text;
            Properties.Settings.Default.Save();
        }

        private void richTextBox3_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.button1 = richTextBox3.Text;
            Properties.Settings.Default.Save();
        }

        private void richTextBox4_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.button2 = richTextBox4.Text;
            Properties.Settings.Default.Save();
        }

        private void neuToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //MessageBox.Show(richTextBox1.Rtf);
            richTextBox1.Rtf = Properties.Settings.Default.Properties["lastcode"].DefaultValue.ToString();
            richTextBox2.Text = Properties.Settings.Default.Properties["title"].DefaultValue.ToString();
            richTextBox3.Text = Properties.Settings.Default.Properties["button1"].DefaultValue.ToString();
            richTextBox4.Text = Properties.Settings.Default.Properties["button2"].DefaultValue.ToString();
            textBox2.Text = "";
            comboBox1.SelectedIndex = -1;
            savepath = "";
        }

        private void speichernToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (savepath == "")
            {
                saveFileDialog1.ShowDialog();
                if (saveFileDialog1.FileName != "")
                {
                    FileStream fs =
                       (FileStream)saveFileDialog1.OpenFile();
                    ASCIIEncoding enc = new ASCIIEncoding();
                    fs.Write(enc.GetBytes(textBox2.Text), 0, textBox2.Text.Length);
                    fs.Close();
                    savepath = saveFileDialog1.FileName;
                    label4.Text = "Datei gespeichert";
                }
            }
            else
            {
                FileStream fs = new FileStream(savepath,FileMode.OpenOrCreate);
                ASCIIEncoding enc = new ASCIIEncoding();
                fs.Write(enc.GetBytes(textBox2.Text), 0, textBox2.Text.Length);
                fs.Close();
                label4.Text = "Datei gespeichert";
            }
        }

        private void speichernUnterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
            if (saveFileDialog1.FileName != "")
            {
                try
                {
                    FileStream fs =
                       (FileStream)saveFileDialog1.OpenFile();
                    ASCIIEncoding enc = new ASCIIEncoding();
                    fs.Write(enc.GetBytes(textBox2.Text), 0, textBox2.Text.Length);
                    fs.Close();
                    savepath = saveFileDialog1.FileName;
                    label4.Text = "Datei gespeichert";
                }
                catch { }
            }
        }

        public static Bitmap RtbToBitmap(RichTextBox rtb)
        {
            rtb.Update();
            Bitmap bmp = new Bitmap(rtb.Width, rtb.Height);
            using (Graphics gr = Graphics.FromImage(bmp))
            {
                gr.CopyFromScreen(rtb.PointToScreen(Point.Empty), Point.Empty, rtb.Size);
            }
            return bmp;
        }

        private void überToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form AboutBox = new AboutBox1();
            AboutBox.Show();
            AboutBox.Focus();
        }

        private void wunschVerbesserungEinsendenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form Infos = new Form2();
            Infos.Show();
            Infos.Focus();
        }
    }
}
