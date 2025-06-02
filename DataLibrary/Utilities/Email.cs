using DataLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DataLibrary
{
    public class Email
    {
        public string EmailAddress { get; set; }
        public string EmailFrom { get; set; }
        public string EmailSubject { get; set; }
        public string EmailTitle { get; set; }
        public string EmailText { get; set; }
        public string VerificationCode { get; set; }

        public string UserName { get; set; }
        public string ComputerName { get; set; }
        public string FullName { get; set; }

        public string errorMsg = "";

        private static Email _Email;

        public static Email Instance
        {
            get
            {
                if (_Email == null)
                {
                    _Email = new Email();
                }
                return _Email;
            }
        }

        public string AddEmailAddress(string emailList, string emailAddress)
        {
            string currentList = Properties.Settings.Default[emailList].ToString();
            switch (currentList.Contains(emailAddress))
            {
                case true:
                    Properties.Settings.Default[emailList] = currentList;
                    break;

                case false:
                    Properties.Settings.Default[emailList] = $"{currentList},{emailAddress}";
                    break;
            }

            return emailAddress;
        }

        public string ResetEmailList(string emailList, string permanentEmail)
        {
            Properties.Settings.Default[emailList] = permanentEmail;
            return null;
        }

        public string SendEmail(Email pEmail, string emailList, bool useAppSettings = false)
        {
            string retVal = "";
            HttpContext _context = HttpContext.Current;

            string emailAddress = useAppSettings ? Properties.Settings.Default[emailList].ToString() : emailList;
            MailMessage NewEmail = new MailMessage("THΣMIS Application NoReply@cwlp.com", emailAddress);
            NewEmail.IsBodyHtml = true;
            NewEmail.Body = pEmail.EmailText;
            NewEmail.Subject = pEmail.EmailSubject.ToString();
            SmtpClient client = new SmtpClient(Properties.Settings.Default["ExchangeIP"].ToString());
            try
            {
                client.Send(NewEmail);
                retVal = "";
            }
            catch (Exception e)
            {
                retVal = "-1";
                errorMsg = e.Message;
            }
            return retVal;
        }
    }


}
