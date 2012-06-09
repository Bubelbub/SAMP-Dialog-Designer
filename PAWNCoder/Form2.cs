using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;

namespace PAWNCoder
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            textBox1.Text = Properties.Settings.Default.email;
            textBox2.Text = "";
            textBox1.SelectionStart = textBox1.Text.Length;
            if (textBox1.Text.Length == 0)
                textBox1.Select();
            else
                textBox2.Select();
            button1.Enabled = false;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            ShoudButtonEnabled();
        }

        private void ShoudButtonEnabled()
        {
            if (textBox2.Text.Length != 0)
            {
                button1.Enabled = true;
            }
            else
            {
                button1.Enabled = false;
            }
            return;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.email = textBox1.Text;
            Properties.Settings.Default.Save();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RetryWeb:
            try
            {
                System.Net.ServicePointManager.Expect100Continue = false;
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://api.gtamylife.de/pawncoder/index.php");
                request.Method = "POST";
                request.UserAgent = "API";
                request.Proxy = new WebProxy();
                string postData = "mail=" + System.Web.HttpUtility.UrlEncode(textBox1.Text) + "&msg=" + System.Web.HttpUtility.UrlEncode(textBox2.Text);
                byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = byteArray.Length;
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                reader.Close();
                dataStream.Close();
                response.Close();

                if (responseFromServer.ToString() == "1")
                {
                    MessageBox.Show("Nachricht erfolgreich versendet.\nVielen Dank für Ihre Verbesserung/Kritik", "Danke", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    textBox2.Text = "";
                }
                else
                {
                    if (MessageBox.Show("Beim Senden der Nachricht ist leider ein Fehler aufgetreten.\nTut uns Leid :(\nDamit dieser gefixt werden kann, Senden Sie bitte manuell eine E-Mail an bubelbub@googlemail.com.\nDanke!", "Danke trotz Fehler", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) == System.Windows.Forms.DialogResult.Retry)
                    {
                        goto RetryWeb;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Diese Fehlermeldung bitte an bubelbub@googlemail.com senden. So können wir das dann fixen.\n" + ex.Message);
            }
        }

        private void textBox1_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.Control && e.KeyValue == 65)
                textBox1.SelectAll();
        }

        private void textBox2_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.Control && e.KeyValue == 65)
                textBox2.SelectAll();
        }
    }
}
