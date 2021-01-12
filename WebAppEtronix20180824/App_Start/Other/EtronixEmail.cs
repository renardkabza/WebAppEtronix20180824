using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Hosting;
using System.Xml;

namespace WebAppEtronix20180824.App_Start.Other
{
    public class EtronixEmail
    {
        private string vIncomingServer = null;
        private string vIncomingPort = null;
        private string vIncoimgServerSSL = null;
        private string vOutgoingServer = null;
        private string vOutgoingPort = null;
        private string vOutgoingServerReqAuthentication = null;
        private string vUserName = null;
        private string vPassword = null;
        private string vappPath = null;
        
        private string vEmailBody = null;
        private string vSendTo = null;

        public EtronixEmail()
        {
            try
            {
                vappPath = HostingEnvironment.ApplicationPhysicalPath;
                vappPath += "\\Settings\\Mail\\EmailServer.xml";

                XmlDocument xml = new XmlDocument();
                xml.Load(vappPath); // suppose that myXmlString contains "<Names>...</Names>"
                XmlNodeList xnList = xml.SelectNodes("//EmailServer");

                foreach (XmlNode xn in xnList)
                {
                    //In
                    vIncomingServer = xn.Attributes["IncomingServer"].Value;
                    vIncomingPort = xn.Attributes["IncomingPort"].Value;
                    vIncoimgServerSSL = xn.Attributes["IncoimgServerSSL"].Value;
                    //Out
                    vOutgoingServer = xn.Attributes["OutgoingServer"].Value;
                    vOutgoingPort = xn.Attributes["OutgoingPort"].Value;
                    vOutgoingServerReqAuthentication = xn.Attributes["OutgoingServerReqAuthentication"].Value;
                    //U&P
                    vUserName = xn.Attributes["UserName"].Value;
                    vPassword = xn.Attributes["Password"].Value;

                }

            }
            catch (Exception e0exception)
            {
                //MessageBox.Show(e0exception.ToString());

            }
        }

        public string VEmailBody
        {
            get { return vEmailBody; }
            set { vEmailBody = value; }
        }

        public string VSendTo
        {
            get { return vSendTo; }
            set { vSendTo = value; }
        }

        public int SendEmail()
        {
            MailMessage mail = new MailMessage();


            try
            {
                mail.To.Add(new MailAddress(vSendTo));
            }
            catch (Exception e2Exception)
            {
                return 1;
                goto koniec;
            }

            /*
            if (CCtxt.Text != "")
            {
                mail.CC.Add(new MailAddress(CCtxt.Text));
            }
            if (BCCtxt.Text != "")
            {
                mail.Bcc.Add(new MailAddress(BCCtxt.Text));
            }
            */
            try
            {
                SmtpClient client = new SmtpClient();
                client.Host = vOutgoingServer;
                client.Port = int.Parse(vOutgoingPort);
                client.EnableSsl = bool.Parse(vOutgoingServerReqAuthentication);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential(vUserName, vPassword);

                mail.From = new MailAddress(vUserName);
                mail.Subject = "Welcome";//Subjecttxt.Text;
                mail.Body = VEmailBody;//"Test1";//Texttxt.Text;

                client.Send(mail);
                //MessageBox.Show("Email Sent");
                return 0;
            }
            catch (Exception e1Exception)
            {
                //MessageBox.Show(e1Exception.ToString());
                return 1;
            }


            koniec:
            ;

            return 0;   
        }

    }
}